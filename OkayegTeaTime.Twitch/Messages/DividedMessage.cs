using System;
using System.Linq;
using System.Threading.Tasks;
using HLE;
using OkayegTeaTime.Files;

namespace OkayegTeaTime.Twitch.Messages;

public readonly struct DividedMessage
{
    private readonly string _channel;
    private readonly string[] _messages;
    private readonly TwitchBot _twitchBot;

    public DividedMessage(TwitchBot twitchBot, string channel, string message)
    {
        _channel = channel;
        _twitchBot = twitchBot;
        _messages = message.Part(AppSettings.MaxMessageLength, ' ').ToArray();
    }

    public void Send()
    {
        Span<string> messages = _messages;
        int messageCount = messages.Length;
        if (messageCount < 2)
        {
            _twitchBot.SendText(_channel, messages[0]);
            return;
        }

        for (int i = 0; i < messageCount; i++)
        {
            _twitchBot.SendText(_channel, messages[i]);
            Task.Delay(AppSettings.DelayBetweenSentMessages).Wait();
        }
    }
}
