using System;

namespace OkayegTeaTimeCSharp.Twitch.Bot.MessageQueue
{
    public class OnMessageAddedToQueueArgs : EventArgs
    {
        public QMessage AddedMessage { get; }

        public OnMessageAddedToQueueArgs(QMessage message)
        {
            AddedMessage = message;
        }
    }
}
