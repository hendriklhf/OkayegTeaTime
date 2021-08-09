using OkayegTeaTimeCSharp.HttpRequests.Enums;

namespace OkayegTeaTimeCSharp.HttpRequests
{
    public class Chatter
    {
        public string Username { get; }

        public ChatRole ChatRole { get; }

        public Chatter(string username, ChatRole chatRole)
        {
            Username = username;
            ChatRole = chatRole;
        }

        public override string ToString()
        {
            return Username;
        }
    }
}
