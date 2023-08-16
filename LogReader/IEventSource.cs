using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinnZan.Utilities
{
    public delegate void SourceEventHandler();

    public interface IEventSource
    {
        event SourceEventHandler Updated;
        List<LogEvent> Events
        {
            get;
        }

        Dictionary<string, string> Watches 
        {
            get;
        }
    }
}
