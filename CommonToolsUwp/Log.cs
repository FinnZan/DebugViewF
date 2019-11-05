using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FinnZan.Utilities
{
    public class CommonTools
    {
        private static object mOutputLock = new object();
        private static bool mLogEnabled = false;
        private static Stopwatch _stopWatch = new Stopwatch();
        private static string mAppName = "FinnZan";
        private static int mLogDepth = 1;
        private static long lastLogTime = 0;

        public static string InitializeDebugger(string appName, int logDepth = 1, bool writeFile = false)
        {
            _stopWatch.Start();

            mLogEnabled = true;
            mAppName = appName.Replace(" ", "_");       
            mLogDepth = logDepth;

            return string.Empty;
        }

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
                    Task.Run(() =>
                    {
                        Output(id, AppDomain.CurrentDomain.FriendlyName, time, strEvent, callstack);
                    });
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

        private static bool Output(int id, string appDomain, long time, string strEvent, string pCallstack)
        {
            string callstack = pCallstack;

            if (callstack.Length <= 0)
            {
                callstack = "none.none";
            }
         
            try
            {
                using (var myChannelFactory = new ChannelFactory<IFinnZanLog>(
                    new NetTcpBinding(SecurityMode.None),
                    new EndpointAddress(new Uri($"net.tcp://localhost/FinnZanLog/{mAppName}"))))
                {
                    var l = myChannelFactory.CreateChannel();
                    l.Log(id + "\t" + appDomain + "\t" + time + "\t" + strEvent + "\t" + callstack);
                }
            }
            catch (Exception ex)
            {
                Debugger.Launch();
            }

            return true;
        }
    }
}
