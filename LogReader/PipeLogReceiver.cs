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
                                e.AppDomain = toks[1];
                                e.Time = toks[2];
                                e.ThreadID = int.Parse(toks[0]);
                                e.Event = toks[3];
                                e.CallStack = CallStackItem.ParseCallStask(toks[4]);
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

        public event SourceEventHandler Updated;
    }
}
