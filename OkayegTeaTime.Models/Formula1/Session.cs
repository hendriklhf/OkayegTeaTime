using System;

namespace OkayegTeaTime.Models.Formula1;

public sealed class Session(string name, DateTime start)
{
    public string Name { get; } = name;

    public DateTime Start { get; } = start;
}
