#nullable disable

using OkayegTeaTimeCSharp.Time;

namespace OkayegTeaTimeCSharp.Database.Models
{
    public partial class Reminder
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

        public Reminder(string fromUser, string toUser, byte[] message, string channel)
        {
            FromUser = fromUser;
            ToUser = toUser;
            Message = message;
            Channel = channel;
        }

        public Reminder(string fromUser, string toUser, byte[] message, string channel, long toTime) : this(fromUser, toUser, message, channel)
        {
            ToTime = toTime;
        }
    }
}
