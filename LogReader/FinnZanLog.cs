using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinnZan.Utilities
{
    internal class FinnZanLog : IFinnZanLog
    {
        public void Log(List<string> logs)
        {
            WcfLogReceiver.Instance.Add(logs);
        }
    }
}
