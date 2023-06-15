using System;

namespace OkayegTeaTime.Twitch.Helix.Models;

[Flags]
public enum EmoteImageFormats
{
    Static = 1 << 0,
    Animated = 1 << 1
}
