using HLE.Strings;
using HLE.Time;

#nullable disable

namespace OkayegTeaTime.Database.EntityFrameworkModels
{
    public class Reminder
    {
        public int Id { get; set; }
        public string Creator { get; set; }
        public string Target { get; set; }
        public byte[] Message { get; set; }
        public string Channel { get; set; }
        public long Time { get; set; } = TimeHelper.Now();
        public long ToTime { get; set; } = 0;

        public Reminder(int id, string creator, string target, byte[] message, string channel, long time, long toTime)
        {
            Id = id;
            Creator = creator;
            Target = target;
            Message = message;
            Channel = channel;
            Time = time;
            ToTime = toTime;
        }

        public Reminder(string creator, string target, byte[] message, string channel, long toTime = 0)
        {
            Creator = creator;
            Target = target;
            Message = message;
            Channel = channel;
            ToTime = toTime;
        }

        public Reminder((string FromUser, string ToUser, string Message, string Channel) values)
        {
            Creator = values.FromUser;
            Target = values.ToUser;
            Message = values.Message.Encode();
            Channel = values.Channel;
        }

        public Reminder((string Creator, string Target, string Message, string Channel, long ToTime) values)
        {
            Creator = values.Creator;
            Target = values.Target;
            Message = values.Message.Encode();
            Channel = values.Channel;
            ToTime = values.ToTime;
        }
    }
}
