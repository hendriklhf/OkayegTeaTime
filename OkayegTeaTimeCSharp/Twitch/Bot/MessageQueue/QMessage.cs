namespace OkayegTeaTimeCSharp.Twitch.Bot.MessageQueue
{
    public class QMessage
    {
        public string Channel { get; }

        public string Message { get; }

        public QMessage(string channel, string message)
        {
            Channel = channel;
            Message = message;
        }
    }
}
