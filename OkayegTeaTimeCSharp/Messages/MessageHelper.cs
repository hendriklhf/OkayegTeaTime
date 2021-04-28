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
