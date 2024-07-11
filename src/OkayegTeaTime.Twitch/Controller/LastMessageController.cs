using System.Collections.Concurrent;
using HLE.Collections;

namespace OkayegTeaTime.Twitch.Controller;

public sealed class LastMessageController
{
    private readonly ConcurrentDictionary<string, string> _lastMessages = new();

    public string this[string channel]
    {
        get => Get(channel);
        set => Set(channel, value);
    }

    private void Set(string channel, string message) => _lastMessages.AddOrSet(channel, message);

    private string Get(string channel)
    {
        if (_lastMessages.TryGetValue(channel, out string? message))
        {
            return message;
        }

        _lastMessages.AddOrSet(channel, string.Empty);
        return string.Empty;
    }
}
