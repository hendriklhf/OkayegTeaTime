using System;
using HLE;
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

        public Reminder(Models.Reminder reminder)
        {
            Creator = reminder.Creator;
            Target = reminder.Target;
            Message = reminder.Message?.Encode() ?? Array.Empty<byte>();
            Channel = reminder.Channel;
            Time = reminder.Time;
            ToTime = reminder.ToTime;
        }
    }
}
