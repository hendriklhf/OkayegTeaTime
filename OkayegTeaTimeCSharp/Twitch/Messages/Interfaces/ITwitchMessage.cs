using System.Drawing;
using OkayegTeaTimeCSharp.Twitch.Messages.Enums;
using TwitchLib.Client.Enums;

namespace OkayegTeaTimeCSharp.Twitch.Messages.Interfaces;

public interface ITwitchMessage : IChatMessage
{
    public List<string> Badges { get; }

    public Color Color { get; }

    public string ColorHex { get; }

    public bool IsTurbo { get; }

    public string RawIrcMessage { get; }

    public int UserId { get; }

    public List<UserTag> UserTags { get; }

    public UserType UserType { get; }
}
