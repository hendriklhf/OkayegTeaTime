using System;

namespace OkayegTeaTime.Twitch.Models.Formula1;

public sealed class Sessionn(string name, DateTime start)
{
    public string Name { get; } = name;

    public DateTime Start { get; } = start;
}
