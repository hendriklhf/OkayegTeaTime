using HLE.Time;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;
using TwitchLib = TwitchLib.Client.Models;

namespace OkayegTeaTime.Twitch.Handlers;

public class Throttler
{
    private readonly Dictionary<string, long> _lastMessages = new();

    public Throttler(TwitchBot twitchBot)
    {
        twitchBot.Channels.ForEach(c => _lastMessages.Add(c, 0));
    }

    public bool CanBeProcessed(TwitchLib::ChatMessage chatMessage)
    {
        long now = TimeHelper.Now();
        if (_lastMessages[chatMessage.Channel] + AppSettings.DelayBetweenReceivedMessages < now)
        {
            _lastMessages[chatMessage.Channel] = now;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CanBeProcessed(TwitchChatMessage chatMessage)
    {
        long now = TimeHelper.Now();
        if (_lastMessages[chatMessage.Channel.Name] + AppSettings.DelayBetweenReceivedMessages < now)
        {
            _lastMessages[chatMessage.Channel.Name] = now;
            return true;
        }
        else
        {
            return false;
        }
    }
}
