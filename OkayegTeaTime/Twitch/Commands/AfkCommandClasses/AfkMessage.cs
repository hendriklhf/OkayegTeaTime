using HLE.Strings;
using HLE.Time;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Files.JsonClasses.CommandData;

namespace OkayegTeaTime.Twitch.Commands.AfkCommandClasses;

public class AfkMessage
{
    public User? User { get; }

    public string? ComingBack { get; private set; }

    public string? GoingAway { get; private set; }

    public string? Resuming { get; private set; }

    public AfkMessage(int userId, string? username = null)
    {
        User = DbControl.Users.GetUser(userId, username);
        CreateMessages();
    }

    private void CreateMessages()
    {
        if (User is null)
        {
            return;
        }

        AfkCommand afkCommand = AppSettings.CommandList[User.AfkType];
        ComingBack = afkCommand.ComingBack.Replace("{username}", User.Username)
            .Replace("{time}", $"{TimeHelper.GetUnixDifference(User.AfkTime)} ago")
            .Replace("{message}", User.AfkMessage);
        ComingBack = string.IsNullOrEmpty(User.AfkMessage) ? ComingBack.Remove(":").ReplaceSpaces() : ComingBack;

        GoingAway = afkCommand.GoingAway.Replace("{username}", User.Username);

        Resuming = afkCommand.Resuming.Replace("{username}", User.Username);
    }
}
