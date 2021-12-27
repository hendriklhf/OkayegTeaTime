#nullable disable

using HLE.Strings;
using HLE.Time;

namespace OkayegTeaTime.Database.Models
{
    public class Reminder
    {
        public int Id { get; set; }
        public string FromUser { get; set; }
        public string ToUser { get; set; }
        public byte[] Message { get; set; }
        public string Channel { get; set; }
        public long Time { get; set; } = TimeHelper.Now();
        public long ToTime { get; set; } = 0;

        public Reminder(int id, string fromUser, string toUser, byte[] message, string channel, long time, long toTime)
        {
            Id = id;
            FromUser = fromUser;
            ToUser = toUser;
            Message = message;
            Channel = channel;
            Time = time;
            ToTime = toTime;
        }

        public Reminder(string fromUser, string toUser, byte[] message, string channel, long toTime = 0)
        {
            FromUser = fromUser;
            ToUser = toUser;
            Message = message;
            Channel = channel;
            ToTime = toTime;
        }

        public Reminder((string FromUser, string ToUser, string Message, string Channel) values)
        {
            FromUser = values.FromUser;
            ToUser = values.ToUser;
            Message = values.Message.Encode();
            Channel = values.Channel;
        }

        public Reminder((string FromUser, string ToUser, string Message, string Channel, long ToTime) values)
        {
            FromUser = values.FromUser;
            ToUser = values.ToUser;
            Message = values.Message.Encode();
            Channel = values.Channel;
            ToTime = values.ToTime;
        }
    }
}
