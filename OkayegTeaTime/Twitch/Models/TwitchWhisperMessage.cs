using System.Drawing;
using HLE.Strings;
using OkayegTeaTime.Twitch.Messages.Interfaces;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Models;

namespace OkayegTeaTime.Twitch.Models;

public class TwitchWhisperMessage : ITwitchWhisperMessage
{
    public int MessageId { get; }

    public int ThreadId { get; }

    public List<string> Badges { get; }

    public Color Color { get; }

    public string ColorHex { get; }

    public bool IsTurbo { get; }

    public string RawIrcMessage { get; }

    public int UserId { get; }

    public UserType UserType { get; }

    public string DisplayName { get; }

    public string[] LowerSplit { get; }

    public string Message { get; }

    public string[] Split { get; }

    public string Username { get; }

    public TwitchWhisperMessage(WhisperMessage whisperMessage)
    {
        Badges = whisperMessage.Badges.Select(b => b.Key).ToList();
        Color = whisperMessage.Color;
        ColorHex = whisperMessage.ColorHex;
        DisplayName = whisperMessage.DisplayName;
        IsTurbo = whisperMessage.IsTurbo;
        Message = whisperMessage.Message.RemoveChatterinoChar().TrimAll();
        LowerSplit = Message.ToLower().Split();
        MessageId = whisperMessage.MessageId.ToInt();
        RawIrcMessage = whisperMessage.RawIrcMessage;
        Split = Message.Split();
        ThreadId = whisperMessage.ThreadId.ToInt();
        UserId = whisperMessage.UserId.ToInt();
        Username = whisperMessage.Username;
        UserType = whisperMessage.UserType;
    }
}
