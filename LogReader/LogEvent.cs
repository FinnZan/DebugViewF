using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinnZan.Utilities
{
    public class LogEvent
    {
        public String Source
        {
            get;
            set;
        }

        public string AppDomain
        {
            get;
            set;
        }

        public int ThreadID
        {
            get;
            set;
        }

        public String Event
        {
            get;
            set;
        }

        public string Time
        {
            get;
            set;
        }

        public CallStackItem[] CallStack
        {
            get;
            set;
        }
    }
}
