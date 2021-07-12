using System;
using System.Collections.Generic;

namespace OkayegTeaTimeCSharp.Twitch.Bot.MessageQueue
{
    public class MessageQueue
    {
        private readonly Dictionary<string, Queue<QMessage>> _queues;

        private readonly Dictionary<string, bool> _sendingState;

        public event EventHandler<OnMessageAddedToQueueArgs> OnMessageAddedToQueue;

        public MessageQueue()
        {
            _queues = new();
            Config.Channels.ForEach(c =>
            {
                _queues.Add(c, new());
            });
            _sendingState = new();
            Config.Channels.ForEach(c =>
            {
                _sendingState.Add(c, false);
            });
        }

        public Queue<QMessage> GetQueue(string channel)
        {
            return _queues[channel];
        }

        public void Enqueue(QMessage message)
        {
            _queues[message.Channel].Enqueue(message);
            OnMessageAddedToQueue.Invoke(this, new(message));
        }

        public QMessage Dequeue(string channel)
        {
            return _queues[channel].Dequeue();
        }

        public bool IsSending(string channel)
        {
            return _sendingState[channel];
        }
    }
}
