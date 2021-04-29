using OkayegTeaTimeCSharp.JsonData;
using OkayegTeaTimeCSharp.Utils;

namespace OkayegTeaTimeCSharp.Messages
{
    public static class MessageHelper
    {
        public static byte[] MakeInsertable(this string input)
        {
            return input.Trim().ReplaceSpaces().EscapeChars().ToByteArray();
        }

        public static bool IsSpecialUser(string username)
        {
            return JsonHelper.JsonToObject().UserLists.SpecialUsers.Contains(username);
        }
    }
}
