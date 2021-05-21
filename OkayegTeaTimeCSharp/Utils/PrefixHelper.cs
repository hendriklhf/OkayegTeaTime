using OkayegTeaTimeCSharp.Database;
using System.Collections.Generic;

namespace OkayegTeaTimeCSharp.Utils
{
    public static class PrefixHelper
    {
        public static Dictionary<string, string> Prefixes { get; private set; } = new();

        public static void FillDictionary()
        {
            Prefixes = DataBase.GetPrefixes();
        }

        public static void Add(string channel)
        {
            Prefixes.Add($"#{channel.Replace("#", "")}", DataBase.GetPrefix($"#{channel.Replace("#", "")}"));
        }

        public static void Update(string channel)
        {
            Prefixes[$"#{channel.Replace("#", "")}"] = DataBase.GetPrefix($"#{channel.Replace("#", "")}");
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