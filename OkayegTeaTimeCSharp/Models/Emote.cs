namespace OkayegTeaTimeCSharp.Models;

public class Emote
{
    public int Index { get; }

    public string Name { get; }

    public Emote(int index, string name)
    {
        Index = index;
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
