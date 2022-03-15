using HLE.Strings;
using HLE.Time;

namespace OkayegTeaTime.Database.Models;

public class Reminder : CacheModel
{
    public int Id { get; }

    public string Creator { get; }

    public string Target { get; }

    public string? Message { get; }

    public string Channel { get; }

    public long Time { get; } = TimeHelper.Now();

    public long ToTime { get; }

    public Reminder(EntityFrameworkModels.Reminder reminder)
    {
        Id = reminder.Id;
        Creator = reminder.Creator;
        Target = reminder.Target;
        Message = reminder.Message?.Decode();
        Channel = reminder.Channel;
        Time = reminder.Time;
        ToTime = reminder.ToTime;
    }

    public Reminder(int id, string creator, string target, string? message, string channel, long toTime = 0)
    {
        Id = id;
        Creator = creator;
        Target = target;
        Message = message;
        Channel = channel;
        ToTime = toTime;
    }
}
