namespace FinnZan.Utilities
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Threading;

    public partial class CommonTools
    {
        private static string _appName = "FinnZan";
        private static bool _logEnabled = false;
        private static ProducerConsumerStream _traceStream = null;
        private static TextWriterTraceListener _traceListener;

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
        public static string InitializeDebugger(string appName)
        {
            _appName = appName.Replace(" ", "_");

            PutLogger();
            
            LoggerCore.Start(_appName, 2);

            if(AttachTraceListener())
            {
                new Thread(
                    () =>
                    {
                        while(_running)
                        {
                            ReadTrace();
                            Thread.Sleep(500);
                        }
                    }).Start();
            }
            else
            {
                LoggerCore.Log("TRACE failed.");
            }

            _logEnabled = true;

            return string.Empty;
        }

        public static void Log(string log, int levelShift = 0)
        {
            if(_logEnabled)
            {
                LoggerCore.Log(log, levelShift);
            }
        }

        public static void HandleException(Exception ex)
        {
            if(_logEnabled)
            {
                LoggerCore.HandleException(ex);
            }
        }

        #region Private

        private static void PutLogger()
        {
            var name = $"{_appName} Log View";

            Process[] processlist = Process.GetProcesses();

            foreach (Process process in processlist)
            {
                if (!String.IsNullOrEmpty(process.MainWindowTitle) && process.MainWindowTitle == name)
                {
                    return;
                }
            }

            try
            {
                var codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                var dllPath = Uri.UnescapeDataString(uri.Path);
                var path = Path.Combine(Path.GetDirectoryName(dllPath), "LogReader.exe");

                File.WriteAllBytes(path, Resources.LogReader);

                var lps = new LaunchProcessFromService();
                lps.LaunchProcess(path, $" RunAsServer {_appName}");       
            }
            catch (Exception ex)
            {
                if (ex.ToString().Contains("LogReader.exe' because it is being used by another process."))
                {
                    LoggerCore.Log("LogReader already running.");
                }
                else
                {
                    LoggerCore.HandleException(ex);
                }
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
                LoggerCore.HandleException(ex);
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

                    LoggerCore.LogTrace(str);
                }
            }
            catch (Exception ex)
            {
                LoggerCore.HandleException(ex);
            }
        }

        #endregion
    }
}
