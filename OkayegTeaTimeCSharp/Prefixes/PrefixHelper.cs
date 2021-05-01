using OkayegTeaTimeCSharp.Database;
using System.Collections.Generic;

namespace OkayegTeaTimeCSharp.Prefixes
{
    public static class PrefixHelper
    {
        public static Dictionary<string, string> Prefixes { get; private set; } = new();

        public static void FillDictionary()
        {
            Prefixes = DataBase.GetPrefixes();
        }

        public static void Update(string channel)
        {
            Prefixes[$"#{channel}"] = DataBase.GetPrefix(channel);
        }

        public static string GetPrefix(string channel)
        {
            try
            {
                return Prefixes[$"#{channel}"] ?? "";
            }
            catch (KeyNotFoundException)
            {
                return "";
            }
        }
    }
}
