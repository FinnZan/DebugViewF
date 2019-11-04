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

        public static CallStackItem[] ParseCallStask(string l)
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

    
    #endregion

}
