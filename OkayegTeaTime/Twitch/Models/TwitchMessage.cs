using System.Drawing;
using HLE.Strings;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Models;

namespace OkayegTeaTime.Twitch.Models;

public class TwitchMessage : ChatMessage
{
    public List<string> Badges { get; }

    public Color Color { get; }

    public string ColorHex { get; }

    public bool IsTurbo { get; }

    public string RawIrcMessage { get; }

    public long UserId { get; }

    public UserType UserType { get; }

    public TwitchMessage(TwitchLibMessage twitchLibMessage)
        : base(twitchLibMessage)
    {
        Badges = twitchLibMessage.Badges.Select(b => b.Key).ToList();
        Color = twitchLibMessage.Color;
        ColorHex = twitchLibMessage.ColorHex;
        IsTurbo = twitchLibMessage.IsTurbo;
        RawIrcMessage = twitchLibMessage.RawIrcMessage;
        UserId = twitchLibMessage.UserId.ToInt();
        UserType = twitchLibMessage.UserType;
    }
}
