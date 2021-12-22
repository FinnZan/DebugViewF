using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace FinnZan.Utilities
{
    internal class LoggerCore
    {
        private static string _appName = "FinnZan";
        private static int _logDepth;

        private static readonly Stopwatch StopWatch = new Stopwatch();
        private static long _lastLogTime = 0;

        private static readonly List<string> OutQueue = new List<string>();
        private static readonly AutoResetEvent WaitHandle = new AutoResetEvent(false);

        public static void Start(string name, int logDepth)
        {
            _appName = name;
            _logDepth = logDepth;

            StopWatch.Start();

            Task.Run(FlushFunc);
        }

        private static void FlushFunc()
        {
            while (true)
            {
                try
                {
                    WaitHandle.WaitOne();

                    List<string> toWrite = new List<string>();

                    lock (OutQueue)
                    {
                        toWrite.AddRange(OutQueue);
                        OutQueue.Clear();
                    }

                    if (toWrite.Count > 0)
                    {
                        using (var myChannelFactory = new ChannelFactory<IFinnZanLog>(new NetTcpBinding(SecurityMode.None), new EndpointAddress(new Uri($"net.tcp://localhost/FinnZanLog/{_appName}"))))
                        {
                            var c = myChannelFactory.CreateChannel();
                            c.Log(toWrite);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Task.Delay(1000);
                }
            }
        }

        public static void LogTrace(string log)
        {
            long time = StopWatch.ElapsedMilliseconds;

            QueueEvent(1, AppDomain.CurrentDomain.FriendlyName, time, $"TRACE [{log.Trim()}]", string.Empty);
        }

        public static void Log(string log, int levelShift = 0)
        {
            lock (OutQueue)
            {
                try
                {
                    var tick = DateTime.Now.Ticks;
                    var nt = StopWatch.ElapsedMilliseconds;
                    var time = (nt - _lastLogTime);
                    _lastLogTime = nt;
                    var id = Thread.CurrentThread.ManagedThreadId;
                    var strEvent = log;

                    // Get source
                    string callStack = "";

                    try
                    {
                        var t = new StackTrace();

                        var frames = t.GetFrames();

                        for (int i = _logDepth + levelShift; i < frames.Length; i++)
                        {
                            try
                            {
                                var frame = frames[i];
                                if (frame != null)
                                {
                                    callStack += "[" + frame.GetMethod().DeclaringType.Name + "." + frame.GetMethod().Name + "]";
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }

                    QueueEvent(id, AppDomain.CurrentDomain.FriendlyName, time, strEvent, callStack);
                }
                catch (Exception ex) { }
            }
        }

        public static void HandleException(Exception ex)
        {
            lock (OutQueue)
            {
                int id = Thread.CurrentThread.ManagedThreadId;
                long time = StopWatch.ElapsedMilliseconds;
                string strEvent = ex.ToString();

                // start output
                QueueEvent(id, AppDomain.CurrentDomain.FriendlyName, time, strEvent, "Exception.Exception");
            }
        }

        private static bool QueueEvent(int id, string appDomain, long time, string strEvent, string pCallstack)
        {
            string callStack = pCallstack;

            if (callStack.Length <= 0)
            {
                callStack = "none.none";
            }

            var strLog = id + "\t" + appDomain + "\t" + time + "\t" + strEvent + "\t" + callStack;

            OutQueue.Add(strLog);

            WaitHandle.Set();

            return true;
        }
    }
}
