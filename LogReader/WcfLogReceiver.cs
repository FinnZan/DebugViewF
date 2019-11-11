using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace FinnZan.Utilities
{
    public class WcfLogReceiver : IEventSource
    {
        private static WcfLogReceiver _instance = null;

        public static WcfLogReceiver Instance 
        {
            get 
            {
                if (_instance == null) 
                {
                    _instance = new WcfLogReceiver();
                }
                return _instance;
            }
        }

        private WcfLogReceiver() 
        {
        }

        public List<LogEvent> Events { get; } = new List<LogEvent>();

        public event SourceEventHandler Updated;

        public void Add(List<string> logs)
        {
            foreach(var log in logs)
            {
                var toks = log.Split('\t');

                LogEvent e = new LogEvent();
                e.AppDomain = toks[1];
                e.Time = toks[2];
                e.ThreadID = int.Parse(toks[0]);
                e.Event = toks[3];
                e.CallStack = CallStackItem.ParseCallStask(toks[4]);
                if(e.CallStack != null)
                {
                    e.Source = $"{e.CallStack[0].Class}.{e.CallStack[0].Method}";
                }
                else
                {
                    e.Source = string.Empty;
                }

                lock(Events)
                {
                    Events.Insert(0, e);
                    Updated();
                }
            }
        }

        public void StartListening(string name) 
        {
            var host = new ServiceHost(typeof(FinnZanLog));
            host.AddServiceEndpoint(typeof(IFinnZanLog), new NetTcpBinding(SecurityMode.None), new Uri($"net.tcp://localhost/FinnZanLog/{name}"));
            host.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
            host.Description.Behaviors.Add(new ServiceDebugBehavior { IncludeExceptionDetailInFaults = true });
            host.Open();
        }
    }

    public class FinnZanLog : IFinnZanLog
    {
        public void Log(List<string> log)
        {
            WcfLogReceiver.Instance.Add(log);            
        }
    }
}
