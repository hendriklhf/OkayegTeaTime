using OkayegTeaTimeCSharp.Utils;
using System.Collections.Generic;
using System.Threading;

namespace OkayegTeaTimeCSharp.Twitch.Bot
{
    public class DividedMessage
    {
        public TwitchBot TwitchBot { get; }

        public string Channel { get; }

        public List<string> Messages { get; }

        public DividedMessage(TwitchBot twitchBot, string channel, string message)
        {
            TwitchBot = twitchBot;
            Channel = channel;
            Messages = message.Split(Config.MaxMessageLength);
        }

        public void StartSending()
        {
            TwitchBot.Send(Channel, Messages[0]);
            Messages.RemoveAt(0);
            if (Messages.Count > 0)
            {
                Thread.Sleep(1300);
                Messages.ForEach(str =>
                {
                    TwitchBot.TwitchClient.SendMessage(Channel, str);
                    Thread.Sleep(1300);
                });
            }
        }
    }
}
