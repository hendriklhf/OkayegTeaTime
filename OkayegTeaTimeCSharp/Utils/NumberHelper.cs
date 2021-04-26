using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkayegTeaTimeCSharp.Utils
{
    public static class NumberHelper
    {
        public static long ToLong(this double d)
        {
            return (long)d;
        }

        public static double ToDouble(this int i)
        {
            return (double)i;
        }
    }
}
