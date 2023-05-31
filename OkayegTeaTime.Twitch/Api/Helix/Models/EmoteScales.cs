using System;

namespace OkayegTeaTime.Twitch.Api.Helix.Models;

[Flags]
public enum EmoteScales
{
    One = 1 << 0,
    Two = 1 << 1,
    Three = 1 << 2
}
