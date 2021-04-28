using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OkayegTeaTimeCSharp.Utils;

namespace OkayegTeaTimeCSharp.Messages
{
    public static class MessageHelper
    {
        public static byte[] Transform(this string input)
        {
            return input.ReplaceSpaces().EscapeChars().ToByteArray();
        }
    }
}
