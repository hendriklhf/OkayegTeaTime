using System.Threading;
using HLE.Strings;
using OkayegTeaTime.Twitch.Bot;

namespace OkayegTeaTime.Twitch.Messages;

public class DividedMessage
{
    public string Channel { get; }

    public string EmoteInFront { get; }

    public List<string> Messages { get; }

    private readonly TwitchBot _twitchBot;

    public DividedMessage(TwitchBot twitchBot, string channel, string emoteInFront, string message)
    {
        Channel = channel;
        _twitchBot = twitchBot;
        EmoteInFront = emoteInFront;
        Messages = message.Split(AppSettings.MaxMessageLength - (EmoteInFront.Length + 1), true).ToList();
    }

    public void StartSending()
    {
        _twitchBot.TwitchClient.SendMessage(Channel, $"{EmoteInFront} {Messages[0]}");
        Messages.RemoveAt(0);
        if (Messages.Count > 0)
        {
            Thread.Sleep(AppSettings.DelayBetweenSentMessages);
            Messages.ForEach(str =>
            {
                _twitchBot.TwitchClient.SendMessage(Channel, str);
                Thread.Sleep(AppSettings.DelayBetweenSentMessages);
            });
        }
    }
}
