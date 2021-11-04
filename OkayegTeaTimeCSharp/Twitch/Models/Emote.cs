namespace OkayegTeaTimeCSharp.Twitch.Models;

public class Emote
{
    public string Name { get; }

    public Emote(string name)
    {
        Name = name;
    }

    public override bool Equals(object obj)
    {
        return obj is Emote emote && emote.Name == Name;
    }

    public override string ToString()
    {
        return Name;
    }
}
