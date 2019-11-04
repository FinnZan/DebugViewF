using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinnZan.Utilities
{
    public enum FilterType
    {
        AppDomain,
        Source,
        Event,
        ThreadID,
    };

    public class Filter
    {
        public Filter(FilterType type, string key)
        {
            Type = type;
            Key = key;
        }

        public FilterType Type
        {
            get; set;
        }
        public string Key
        {
            get; set;
        }
    }

}
