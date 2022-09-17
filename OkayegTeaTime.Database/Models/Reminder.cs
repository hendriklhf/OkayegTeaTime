using HLE.Time;

namespace OkayegTeaTime.Database.Models;

public sealed class Reminder : CacheModel
{
    public int Id { get; internal set; }

    public string Creator { get; }

    public string Target { get; }

    public string? Message { get; }

    public string Channel { get; }

    public long Time { get; } = TimeHelper.Now();

    public long ToTime { get; }

    public bool HasBeenSent { get; set; }

    public Reminder(EntityFrameworkModels.Reminder reminder)
    {
        Id = reminder.Id;
        Creator = reminder.Creator;
        Target = reminder.Target;
        Message = reminder.Message;
        Channel = reminder.Channel;
        Time = reminder.Time;
        ToTime = reminder.ToTime;
    }

    public Reminder(string creator, string target, string? message, string channel, long toTime = 0)
    {
        Creator = creator;
        Target = target;
        Message = message;
        Channel = channel;
        ToTime = toTime;
    }
}
