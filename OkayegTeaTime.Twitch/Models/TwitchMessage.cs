using TwitchLib.Client.Models;

namespace OkayegTeaTime.Twitch.Models;

public class TwitchMessage : ChatMessage
{
    public long UserId { get; }

    public TwitchMessage(TwitchLibMessage message) : base(message)
    {
        UserId = long.Parse(message.UserId);
    }
}
