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
            get; private set;
        }

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
                        e.ThreadID = int.Parse(toks[0]);
                        e.Event = toks[2];                        
                        e.CallStack = ParseCallStask(toks[3]);
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
    }
}
