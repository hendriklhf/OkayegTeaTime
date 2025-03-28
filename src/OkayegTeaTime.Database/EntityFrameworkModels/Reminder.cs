﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace OkayegTeaTime.Database.EntityFrameworkModels;

public sealed class Reminder
{
    public int Id { get; set; }

    public string Creator { get; set; }

    public string Target { get; set; }

    public string? Message { get; set; }

    public string Channel { get; set; }

    [SuppressMessage("Major Code Smell", "S6354:Use a testable date/time provider")]
    public long Time { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    public long ToTime { get; set; }

    public Reminder(int id, string creator, string target, string message, string channel, long time, long toTime)
    {
        Id = id;
        Creator = creator;
        Target = target;
        Message = message;
        Channel = channel;
        Time = time;
        ToTime = toTime;
    }

    public Reminder(string creator, string target, string message, string channel, long toTime = 0)
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
        Message = reminder.Message;
        Channel = reminder.Channel;
        Time = reminder.Time;
        ToTime = reminder.ToTime;
    }
}
