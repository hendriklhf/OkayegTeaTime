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
        _lastMessages = DbControl.Channels.Select(c => c.Name).ToDictionary(c => c, _ => string.Empty);
    }

    private void Add(string channel, string message = "")
    {
        if (!_lastMessages.ContainsKey(channel))
        {
            _lastMessages.Add(channel, message);
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
        if (_lastMessages.TryGetValue(channel, out string? message))
        {
            return message;
        }

        Add(channel);
        return string.Empty;
    }
}
