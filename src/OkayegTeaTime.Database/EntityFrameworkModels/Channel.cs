namespace OkayegTeaTime.Database.EntityFrameworkModels;

public sealed class Channel
{
    public long Id { get; set; }

    public string Name { get; set; }

    public string? EmoteInFront { get; set; }

    public string? Prefix { get; set; }

    public Channel(long id, string name, string? emoteInFront, string? prefix)
    {
        Id = id;
        Name = name;
        EmoteInFront = emoteInFront;
        Prefix = prefix;
    }

    public Channel(long id, string name)
    {
        Id = id;
        Name = name;
    }
}
