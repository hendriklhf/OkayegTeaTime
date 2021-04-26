using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkayegTeaTimeCSharp.Utils
{
    public static class StringHelper
    {
        public static byte[] ToByteArray(this string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        public static string EscapeChars(this string str)
        {
            return str.Replace("\\", "\\\\").Replace("'", "\\'");
        }
    }
}
