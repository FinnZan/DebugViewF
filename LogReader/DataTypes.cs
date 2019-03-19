using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinnZan.Utilities
{
    #region Sub classes ========================

    public class CallStackItem
    {
        public CallStackItem() { }

        public CallStackItem(StackFrame sf)
        {
            try
            {
                Class = sf.GetMethod().DeclaringType.FullName;
                Method = sf.GetMethod().Name;
                Parameters = string.Empty;
                foreach (var p in sf.GetMethod().GetParameters())
                {
                    Parameters += "[" + p + "]";
                }
            }
            catch (Exception ex)
            {

            }
        }

        public string Class
        {
            get;
            set;
        }

        public string Method
        {
            get;
            set;
        }

        public string Parameters
        {
            get;
            set;
        }
    }

    public class LogEvent
    {
        public String Source
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

    public enum FilterType
    {
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

    #endregion

}
