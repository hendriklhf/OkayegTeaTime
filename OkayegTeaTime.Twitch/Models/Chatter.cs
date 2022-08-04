namespace OkayegTeaTime.Twitch.Models;

public class Chatter
{
    public string Username { get; }

    public ChatRole ChatRole { get; }

    public Chatter(string username, ChatRole chatRole)
    {
        Username = username;
        ChatRole = chatRole;
    }

    public override bool Equals(object? obj)
    {
        return obj is Chatter chatter && chatter.Username == Username;
    }

    public override string ToString()
    {
        return Username;
    }
}
