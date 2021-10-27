namespace OkayegTeaTimeCSharp.Twitch.Bot;

public static class LastMessagesDictionary
{
    private static Dictionary<string, string> _lastMessages = new();

    public static void FillDictionary()
    {
        _lastMessages = Config.Channels.ToDictionary(c => c, c => string.Empty);
    }

    public static void Add(string channel, string message = "")
    {
        if (!_lastMessages.ContainsKey(channel))
        {
            _lastMessages.Add(channel, message);
        }
    }

    public static void Set(string channel, string message)
    {
        if (_lastMessages.ContainsKey(channel))
        {
            _lastMessages[channel] = message;
        }
        else
        {
            Add(channel, message);
        }
    }

    public static string Get(string channel)
    {
        if (_lastMessages.TryGetValue(channel, out string message))
        {
            return message;
        }
        else
        {
            return null;
        }
    }
}
