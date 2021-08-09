namespace OkayegTeaTimeCSharp.HttpRequests;

public class Chatter
{
    public string Username { get; }
    
    public ChatRole ChatRole { get; }
    
    public Chatter(string username, ChatRole chatRole)
    {
        Username = username;
        ChatRole = charRole;
    }
}
