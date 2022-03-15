using System.Threading;
using HLE.Strings;
using OkayegTeaTime.Twitch.Bot;

namespace OkayegTeaTime.Twitch.Messages;

public class DividedMessage
{
    public TwitchBot TwitchBot { get; }

    public string Channel { get; }

    public string EmoteInFront { get; }

    public List<string> Messages { get; }

    public DividedMessage(TwitchBot twitchBot, string channel, string emoteInFront, string message)
    {
        Channel = channel;
        TwitchBot = twitchBot;
        EmoteInFront = emoteInFront;
        Messages = message.Split(AppSettings.MaxMessageLength - (EmoteInFront.Length + 1), true).ToList();
    }

    public void StartSending()
    {
        TwitchBot.TwitchClient.SendMessage(Channel, $"{EmoteInFront} {Messages[0]}");
        Messages.RemoveAt(0);
        if (Messages.Count > 0)
        {
            Thread.Sleep(AppSettings.DelayBetweenSentMessages);
            Messages.ForEach(str =>
            {
                TwitchBot.TwitchClient.SendMessage(Channel, str);
                Thread.Sleep(AppSettings.DelayBetweenSentMessages);
            });
        }
    }
}
