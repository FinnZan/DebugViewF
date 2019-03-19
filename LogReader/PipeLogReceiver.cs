using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace FinnZan.Utilities
{
    public class PipeLogReceiver : IEventSource
    {
        private List<LogEvent> _logs = new List<LogEvent>();

        public List<LogEvent> Events
        {
            get
            {
                return _logs;
            }
        }

        public bool StartListening(string name)
        {
            try
            {
                var timer = new DispatcherTimer();
                timer.Tick += new EventHandler((object sender, EventArgs e) =>
                {
                    _logs.Add(new LogEvent() { Source = "timer", Event = "message", ThreadID = 0, Time = DateTime.Now.ToLongTimeString() });
                    Updated();
                });
                timer.Interval = TimeSpan.FromSeconds(1);
                //timer.Start();               

                new Thread(() =>
                {
                    PipeSecurity pipeSa = new PipeSecurity();
                    pipeSa.AddAccessRule(new PipeAccessRule("Everyone",
                                                        PipeAccessRights.ReadWrite,
                                                        System.Security.AccessControl.AccessControlType.Allow));

                    var server = new NamedPipeServerStream($"FinnZan_{name}",
                                                            PipeDirection.InOut,
                                                            4,
                                                            PipeTransmissionMode.Byte,
                                                            PipeOptions.None,
                                                            1024,
                                                            1024,
                                                            pipeSa,
                                                            HandleInheritability.Inheritable);

                    StreamReader reader = new StreamReader(server);

                    while (true)
                    {
                        try
                        {
                            server.WaitForConnection();
                            Trace.Write("connected.");
                            server.WaitForPipeDrain();
                            string line = reader.ReadToEnd();
                            if (line != null)
                            {
                                var toks = line.Split('\t');

                                LogEvent e = new LogEvent();
                                e.Time = toks[1];
                                e.ThreadID = int.Parse(toks[0]);
                                e.Event = toks[2];
                                e.CallStack = ParseCallStask(toks[3]);
                                if (e.CallStack != null)
                                {
                                    e.Source = $"{e.CallStack[0].Class}.{e.CallStack[0].Method}";
                                }
                                else
                                {
                                    e.Source = string.Empty;
                                }
                                _logs.Insert(0, e);

                                Updated();
                            }
                        }
                        catch (IOException ex)
                        {
                            Trace.Write("Disconnected.");
                            server.Disconnect();
                        }
                        catch (Exception ex)
                        {
                            Trace.Write(ex);
                            Thread.Sleep(2000);
                        }
                    }
                }).Start();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        private CallStackItem[] ParseCallStask(string l)
        {
            char[] splitors = { '[', ']', ' ' };
            List<CallStackItem> items = new List<CallStackItem>();

            try
            {
                var toks = l.Split(splitors);

                foreach (var t in toks.Where(o => o.Length > 0))
                {
                    try
                    {
                        CallStackItem c = new CallStackItem();
                        var toks2 = t.Split('.');
                        c.Class = toks2[0];
                        c.Method = toks2[1];

                        items.Add(c);
                    }
                    catch (Exception ex)
                    {

                    }
                }

                return items.ToArray();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public event SourceEventHandler Updated;
    }
}
