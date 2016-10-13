using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnifiedFrameWork.UnifiedComponents
{
    public class SpecialCharHandler
    {
        public static string QuotesCleanser(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                var tempValue = value.Replace("\"", "'");
                return tempValue;
            }
            return value;
        }

        public static string EscapeCharCleanser(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                var tempValue = value.Replace("\"", "\'");
                return tempValue;
            }
            return value;
        }
    }
}
