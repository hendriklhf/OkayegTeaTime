using System.Threading;
using HLE.Strings;
using OkayegTeaTimeCSharp.Twitch.Bot;

namespace OkayegTeaTimeCSharp.Twitch.Messages
{
    public class DividedMessage
    {
        public TwitchBot TwitchBot { get; }

        public string Channel { get; }

        public string EmoteInFront { get; }

        public List<string> Messages { get; }

        public DividedMessage(TwitchBot twitchBot, string channel, string emoteInFront, string message)
        {
            TwitchBot = twitchBot;
            Channel = channel;
            EmoteInFront = emoteInFront;
            Messages = message.Split(TwitchConfig.MaxMessageLength - (EmoteInFront.Length + 1));
        }

        public void StartSending()
        {
            TwitchBot.TwitchClient.SendMessage(Channel, $"{EmoteInFront} {Messages[0]}");
            Messages.RemoveAt(0);
            if (Messages.Count > 0)
            {
                Thread.Sleep(TwitchConfig.MinDelayBetweenMessages);
                Messages.ForEach(str =>
                {
                    TwitchBot.TwitchClient.SendMessage(Channel, str);
                    Thread.Sleep(TwitchConfig.MinDelayBetweenMessages);
                });
            }
        }
    }
}
