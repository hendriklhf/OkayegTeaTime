using System;

namespace OkayegTeaTime.Twitch.Helix.Models;

[Flags]
public enum EmoteScales
{
    None = 0,
    One = 1 << 0,
    Two = 1 << 1,
    Three = 1 << 2
}
