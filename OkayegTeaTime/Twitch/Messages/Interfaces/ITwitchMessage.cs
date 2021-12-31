using System.Drawing;
using TwitchLib.Client.Enums;

namespace OkayegTeaTime.Twitch.Messages.Interfaces;

public interface ITwitchMessage : IChatMessage
{
    public List<string> Badges { get; }

    public Color Color { get; }

    public string ColorHex { get; }

    public bool IsTurbo { get; }

    public string RawIrcMessage { get; }

    public int UserId { get; }

    public UserType UserType { get; }
}
