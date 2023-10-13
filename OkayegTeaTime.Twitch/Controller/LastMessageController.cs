using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using HLE.Collections;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Cache;

namespace OkayegTeaTime.Twitch.Controller;

public sealed class LastMessageController
{
    private readonly ConcurrentDictionary<string, string> _lastMessages;

    public LastMessageController(ChannelCache? channelCache = null)
    {
        Dictionary<string, string> lastMessages = channelCache is null ? DbController.GetChannels().Select(static c => c.Name).ToDictionary(static c => c, static _ => string.Empty) : channelCache.Select(static c => c.Name).ToDictionary(static c => c, static _ => string.Empty);
        _lastMessages = new(lastMessages);
    }

    public string this[string channel]
    {
        get => Get(channel);
        set => Set(channel, value);
    }

    private void Set(string channel, string message)
    {
        if (_lastMessages.TryAdd(channel, message))
        {
            return;
        }

        _lastMessages[channel] = message;
    }

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
