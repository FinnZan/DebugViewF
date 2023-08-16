using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinnZan.Utilities
{
    public class LogFileReader : IEventSource
    {
        public List<LogEvent> Events
        {
            get; protected set;
        }

        public Dictionary<string, string> Watches => new Dictionary<string, string>();

        public event SourceEventHandler Updated;

        public List<LogEvent> LoadFile(string filename)
        {
            Events = new List<LogEvent>();
            try
            {
                var lines = File.ReadAllLines(filename);
                foreach (var l in lines)
                {
                    try
                    {
                        var toks = l.Split('\t');

                        LogEvent e = new LogEvent();
                        e.Time = toks[1];
                        e.AppDomain = toks[2];
                        e.ThreadID = int.Parse(toks[0]);
                        e.Event = toks[3];                        
                        e.CallStack = CallStackItem.ParseCallStask(toks[4]);
                        e.Source = $"{e.CallStack[0].Class}.{e.CallStack[0].Method}";
                        Events.Insert(0, e);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                Updated();

                return Events;
            }
            catch (Exception ex)
            {                
                return null;
            }            
        }
    }
}
