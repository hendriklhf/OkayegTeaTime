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

    public TwitchMessage(TwitchLibMessage message)
        : base(message)
    {
        Badges = message.Badges.Select(b => b.Key).ToArray();
        Color = message.Color;
        ColorHex = message.ColorHex;
        IsTurbo = message.IsTurbo;
        RawIrcMessage = message.RawIrcMessage;
        UserId = long.Parse(message.UserId);
        UserType = message.UserType;
    }
}
