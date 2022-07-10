using System;

namespace OkayegTeaTime.Files.Models;

public class Formula1Session
{
    public string Name { get; }

    public DateTime Start { get; }

    public Formula1Session(string name, DateTime start)
    {
        Name = name;
        Start = start;
    }
}
