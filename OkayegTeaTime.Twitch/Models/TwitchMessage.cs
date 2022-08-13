using System.Drawing;
using System.Linq;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Models;

namespace OkayegTeaTime.Twitch.Models;

public class TwitchMessage : ChatMessage
{
    public string[] Badges { get; }

    public Color Color { get; }

    public string ColorHex { get; }

    public bool IsTurbo { get; }

    public string RawIrcMessage { get; }

    public long UserId { get; }

    public UserType UserType { get; }

    public TwitchMessage(TwitchLibMessage twitchLibMessage)
        : base(twitchLibMessage)
    {
        Badges = twitchLibMessage.Badges.Select(b => b.Key).ToArray();
        Color = twitchLibMessage.Color;
        ColorHex = twitchLibMessage.ColorHex;
        IsTurbo = twitchLibMessage.IsTurbo;
        RawIrcMessage = twitchLibMessage.RawIrcMessage;
        UserId = long.Parse(twitchLibMessage.UserId);
        UserType = twitchLibMessage.UserType;
    }
}
