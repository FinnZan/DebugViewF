namespace FinnZan.Utilities
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Pipes;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Windows;

    public partial class CommonTools
    {
        private static string mAppName = "FinnZan";
        private static bool mLogEnabled = false;
        private static int mLogDepth = 1;
        private static bool mWriteFile = false;
        private static string mLogFilePath = @"RadiumLog.txt";
        private static ProducerConsumerStream _traceStream = null;
        private static Stopwatch _stopWatch = new Stopwatch();
        private static TextWriterTraceListener _traceListener;
        private static object mOutputLock = new object();
        private static bool _running = true;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="app">The "host" app</param>
        /// <param name="logDepth">
        ///     How many layaer up on the stack we should go so we can show the actual method we are interested in 
        /// </param>
        /// <param name="writeFile"></param>        
        public static string InitializeDebugger(string appName, int logDepth = 1, bool writeFile = false)
        {
            _stopWatch.Start();

            mLogEnabled = true;
            mAppName = appName.Replace(" ", "_");
            mWriteFile = writeFile;
            mLogFilePath = Path.GetFullPath(appName + "Log.txt");
            mLogDepth = logDepth;

            var ret = PutLogger();

            if (ret == "OK")
            {
                if (AttachTraceListener())
                {
                    new Thread(() =>
                    {
                        while (_running)
                        {
                            ReadTrace();
                            Thread.Sleep(500);
                        }
                    }).Start();
                }
                else
                {
                    CommonTools.Log("TRACE failed.");
                }
            }

            return ret;
        }

        private static long lastLogTime = 0;

        public static void Log(string log, int levelShift = 0)
        {
            lock (mOutputLock)
            {
                if (!mLogEnabled)
                {
                    return;
                }

                try
                {
                    long tick = DateTime.Now.Ticks;
                    var nt = _stopWatch.ElapsedMilliseconds;
                    long time = (nt - lastLogTime);
                    lastLogTime = nt;
                    int id = Thread.CurrentThread.ManagedThreadId;
                    string source = string.Empty;
                    string strEvent = log;                    

                    try
                    {
                        strEvent = string.Format(log);
                    }
                    catch (Exception ex)
                    {
                        strEvent = log;
                    }

                    // Get source
                    string callstack = "";

                    try
                    {
                        StackTrace t = new StackTrace();
                        StackFrame frame = t.GetFrame(mLogDepth + levelShift);
                        if (frame != null)
                        {
                            source = frame.GetMethod().DeclaringType.Name + "." + frame.GetMethod().Name;
                        }

                        for (int i = mLogDepth + levelShift; i < t.GetFrames().Length; i++)
                        {
                            try
                            {
                                frame = t.GetFrame(i);
                                if (frame != null)
                                {
                                    callstack += "[" + frame.GetMethod().DeclaringType.Name + "." + frame.GetMethod().Name + "]";
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        source = "Error";
                    }

                    // Start output 
                    Output(id, AppDomain.CurrentDomain.FriendlyName, time, strEvent, callstack);

                }
                catch (Exception ex) { }
            }
        }

        public static void HandleException(Exception ex)
        {
            if (!mLogEnabled)
            {
                return;
            }

            int id = Thread.CurrentThread.ManagedThreadId;
            long time = _stopWatch.ElapsedMilliseconds;
            string source = string.Empty;
            string strEvent = ex.ToString();

            // Get source
            StackTrace t = new StackTrace();
            try
            {
                StackFrame frame = t.GetFrame(1);
                if (frame != null)
                {
                    source = frame.GetMethod().DeclaringType.Name + "." + frame.GetMethod().Name;
                    strEvent += " - " + frame.GetFileName() + "(" + frame.GetFileLineNumber() + ")";
                }
            }
            catch (Exception exc)
            {
                source = "Error";
            }

            // start output
            Output(id, AppDomain.CurrentDomain.FriendlyName, time, strEvent, string.Empty);
        }

        private static string PutLogger()
        {
            try
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string dllpath = Uri.UnescapeDataString(uri.Path);
                string path = Path.Combine(Path.GetDirectoryName(dllpath), "LogReader.exe");

                File.WriteAllBytes(path, Resources.LogReader);

                LaunchProcessFromService lps = new LaunchProcessFromService();
                lps.LaunchProcess(path, $" RunAsServer {mAppName}");
                //Process.Start(path, $" RunAsServer {mAppName}");                                

                return "OK";
            }
            catch (Exception ex)
            {
                CommonTools.HandleException(ex);
                return ex.ToString();
            }
        }

        private static bool AttachTraceListener()
        {
            try
            {
                _traceStream = new ProducerConsumerStream();

                _traceListener = new TextWriterTraceListener(_traceStream);
                Trace.Listeners.Clear();
                Trace.Listeners.Add(_traceListener);

                return true;
            }
            catch (Exception ex)
            {
                CommonTools.HandleException(ex);
                return false;
            }
        }

        private static void UnInitializeDebugger()
        {
            _running = false;
            _traceStream?.Dispose();
            _traceListener?.Dispose();
        }

        private static void ReadTrace()
        {
            try
            {
                Trace.Flush();
                int count = 0;
                byte[] buffer = new byte[_traceStream.Length];
                count = _traceStream.Read(buffer, 0, (int)_traceStream.Length);                

                if (count > 0)
                {
                    string str = string.Empty;

                    if (count == _traceStream.Length)
                    {
                        str = Encoding.ASCII.GetString(buffer).Trim();
                    }
                    else
                    {
                        byte[] sub = new byte[count];
                        Array.Copy(buffer, 0, sub, 0, count);
                        str = Encoding.ASCII.GetString(sub).Trim();
                    }
                    
                    long time = _stopWatch.ElapsedMilliseconds;

                    Output(1, AppDomain.CurrentDomain.FriendlyName, time, $"TRACE [{str.Trim()}]", string.Empty);                 
                }
            }
            catch (Exception ex)
            {
                CommonTools.HandleException(ex);
            }
        }

        private static bool Output(int id, string appDomain, long time, string strEvent, string pCallstack)
        {
            string callstack = pCallstack;

            if (callstack.Length <= 0)
            {
                callstack = "none.none";
            }

            if (mWriteFile)
            {
                // Local file
                try
                {
                    using (StreamWriter sw = File.AppendText(mLogFilePath))
                    {
                        sw.WriteLine(id + "\t" + time + "\t" + strEvent + "\t" + callstack);
                    }

                }
                catch (Exception ex)
                {
                    //CommonTools.HandleException(ex);
                }
            }

            try
            {
                NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", $"FinnZan_{mAppName}", PipeDirection.InOut, PipeOptions.None);
                pipeClient.Connect(3 * 1000);
                StreamWriter writer = new StreamWriter(pipeClient);
                writer.WriteLine(id + "\t" + appDomain + "\t"+ time + "\t" + strEvent + "\t" + callstack);
                writer.Flush();
                pipeClient.Close();
            }
            catch (Exception ex)
            {
            }

            return true;
        }
    }
}
