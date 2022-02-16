using HLE.Strings;
using HLE.Time;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Files.JsonClasses.CommandData;
using OkayegTeaTime.Twitch.Commands.Enums;

namespace OkayegTeaTime.Twitch.Commands.AfkCommandClasses;

public class AfkMessage
{
    public UserNew? User { get; }

    public string? ComingBack { get; private set; }

    public string? GoingAway { get; private set; }

    public string? Resuming { get; private set; }

    public AfkMessage(int userId, string? username = null)
    {
        User = DbController.GetUser(userId, username);
        CreateMessages();
    }

    private void CreateMessages()
    {
        if (User is null)
        {
            return;
        }

        AfkCommand afkCommand = AppSettings.CommandList[(AfkCommandType)User.AfkType];
        ComingBack = afkCommand.ComingBack.Replace("{username}", User.Username)
            .Replace("{time}", $"{TimeHelper.GetUnixDifference(User.AfkTime)} ago")
            .Replace("{message}", User.AfkMessage.Decode());
        ComingBack = string.IsNullOrEmpty(User.AfkMessage.Decode()) ? ComingBack.Remove(":").ReplaceSpaces() : ComingBack;

        GoingAway = afkCommand.GoingAway.Replace("{username}", User.Username);

        Resuming = afkCommand.Resuming.Replace("{username}", User.Username);
    }
}
