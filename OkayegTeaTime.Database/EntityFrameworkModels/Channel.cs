namespace OkayegTeaTime.Database.EntityFrameworkModels;

public class Channel
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string? EmoteInFront { get; set; }
    public string? Prefix { get; set; }

    public Channel(long id, string name, string? emote = null, string? prefix = null)
    {
        Id = id;
        Name = name;
        EmoteInFront = emote;
        Prefix = prefix;
    }
}
