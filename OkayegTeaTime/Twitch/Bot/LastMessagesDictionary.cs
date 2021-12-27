using OkayegTeaTime.Database;

namespace OkayegTeaTime.Twitch.Bot;

public class LastMessagesDictionary
{
    private Dictionary<string, string> _lastMessages = new();

    public LastMessagesDictionary()
    {
        FillDictionary();
    }

    public string this[string channel]
    {
        get => Get(channel);
        set => Set(channel, value);
    }

    private void FillDictionary()
    {
        _lastMessages = DatabaseController.GetChannels().ToDictionary(c => c, c => string.Empty);
    }

    public void Add(string channel, string message = null)
    {
        if (!_lastMessages.ContainsKey(channel))
        {
            _lastMessages.Add(channel, message ?? string.Empty);
        }
    }

    private void Set(string channel, string message)
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

    private string Get(string channel)
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
