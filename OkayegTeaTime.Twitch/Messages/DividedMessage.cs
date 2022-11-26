using System.Linq;
using System.Threading;
using HLE;
using HLE.Collections;
using OkayegTeaTime.Files;

namespace OkayegTeaTime.Twitch.Messages;

public sealed class DividedMessage
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

    public void StartSending()
    {
        _twitchBot.SendText(_channel, _messages[0]);
        if (_messages.Length < 2)
        {
            return;
        }

        _messages[1..].ForEach(str =>
        {
            Thread.Sleep(AppSettings.DelayBetweenSentMessages);
            _twitchBot.SendText(_channel, str);
        });
    }
}
