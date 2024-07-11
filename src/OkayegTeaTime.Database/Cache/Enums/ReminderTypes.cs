using System;

namespace OkayegTeaTime.Database.Cache.Enums;

[Flags]
public enum ReminderTypes
{
    None = 0,
    Timed = 1,
    NonTimed = 2
}
