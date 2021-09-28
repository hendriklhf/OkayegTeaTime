using OkayegTeaTimeCSharp.Database;

namespace OkayegTeaTimeCSharp.Twitch.Bot
{
    public static class PrefixDictionary
    {
        private static Dictionary<string, string> _prefixes = new();

        public static void Add(string channel)
        {
            if (!_prefixes.ContainsKey(channel))
            {
                _prefixes.Add(channel, DataBase.GetPrefix(channel));
            }
        }

        public static void FillDictionary()
        {
            _prefixes = DataBase.GetPrefixes();
        }

        public static string Get(string channel)
        {
            if (_prefixes.TryGetValue(channel, out string prefix))
            {
                if (!string.IsNullOrEmpty(prefix))
                {
                    return prefix;
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return string.Empty;
            }
        }

        public static void Update(string channel)
        {
            _prefixes[channel] = DataBase.GetPrefix(channel);
        }
    }
}
