using System;

namespace OkayegTeaTime.Database.Cache.Enums;

[Flags]
public enum ReminderType : byte
{
    Timed = 1,
    NonTimed = 2
}
