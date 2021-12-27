using System.Drawing;
using HLE.Strings;
using OkayegTeaTime.Files.JsonClasses.Settings;
using OkayegTeaTime.Twitch.Messages.Enums;
using OkayegTeaTime.Twitch.Messages.Interfaces;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Models;

namespace OkayegTeaTime.Twitch.Models;

public class TwitchMessage : ITwitchMessage
{
    public List<string> Badges { get; }

    public Color Color { get; }

    public string ColorHex { get; }

    public bool IsTurbo { get; }

    public string RawIrcMessage { get; }

    public int UserId { get; }

    public List<UserTag> UserTags { get; }

    public UserType UserType { get; }

    public string DisplayName { get; }

    public string[] LowerSplit { get; }

    public string Message { get; }

    public string[] Split { get; }

    public string Username { get; }

    public TwitchMessage(TwitchLibMessage twitchLibMessage)
    {
        Badges = twitchLibMessage.Badges.Select(b => b.Key).ToList();
        Color = twitchLibMessage.Color;
        ColorHex = twitchLibMessage.ColorHex;
        DisplayName = twitchLibMessage.DisplayName;
        IsTurbo = twitchLibMessage.IsTurbo;
        RawIrcMessage = twitchLibMessage.RawIrcMessage;
        Message = GetMessage();
        LowerSplit = GetLowerSplit();
        Split = GetSplit();
        UserId = twitchLibMessage.UserId.ToInt();
        Username = twitchLibMessage.Username;
        UserTags = GetUserTags();
        UserType = twitchLibMessage.UserType;
    }

    private List<UserTag> GetUserTags()
    {
        List<UserTag> result = new() { UserTag.Normal };
        UserLists userLists = AppSettings.UserLists;
        if (userLists.Moderators.Contains(Username))
        {
            result.Add(UserTag.Moderator);
        }
        if (userLists.Owners.Contains(Username))
        {
            result.Add(UserTag.Owner);
        }
        if (userLists.IgnoredUsers.Contains(Username))
        {
            result.Add(UserTag.Special);
        }
        if (userLists.SecretUsers.Contains(Username))
        {
            result.Add(UserTag.Secret);
        }
        return result;
    }

    private string GetMessage()
    {
        string message = RawIrcMessage.Match(@"(WHISPER|PRIVMSG)\s#?\w+\s:.+$");
        return message.ReplacePattern(@"^(WHISPER|PRIVMSG)\s#?\w+\s:", "").RemoveChatterinoChar().TrimAll();
    }

    private string[] GetSplit()
    {
        return Message.Split();
    }

    private string[] GetLowerSplit()
    {
        return Message.ToLower().Split();
    }
}
