using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileHeaderChecker
{
    public class CodeFile
    {
        public string FullPath { get; set; }
        public DateTimeOffset? Start { get; set; }
        public DateTimeOffset? End { get; set; }
        public string Author { get; set; }

        public string GetYearString() 
        {
            if (Start.HasValue && End.HasValue)
            {
                if (Start.Value.Year == End.Value.Year)
                {
                    return $"{Start.Value.Year}";
                }
                else 
                {
                    return $"{Start.Value.Year}-{End.Value.Year}";
                }
            }
            else
            {
                return DateTime.Now.Year.ToString();
            }
        }
    }
}
