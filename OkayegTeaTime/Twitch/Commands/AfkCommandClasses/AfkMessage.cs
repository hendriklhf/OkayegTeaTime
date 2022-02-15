using HLE.Strings;
using HLE.Time;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Files.JsonClasses.CommandData;
using OkayegTeaTime.Twitch.Handlers;

namespace OkayegTeaTime.Twitch.Commands.AfkCommandClasses;

public class AfkMessage
{
    public string ComingBack { get; private set; }

    public string GoingAway { get; private set; }

    public string Resuming { get; private set; }

    public AfkMessage(User user)
    {
        string type = user.Type.ToLower();
        AfkCommand afkCommand = AppSettings.CommandList[CommandHandler.AfkCommandTypes.FirstOrDefault(t => t.ToString().ToLower() == type)];
        ComingBack = afkCommand.ComingBack;
        GoingAway = afkCommand.GoingAway;
        Resuming = afkCommand.Resuming;
        ReplaceSpaceHolder(user);
    }

    private void ReplaceSpaceHolder(User user)
    {
        ComingBack = ComingBack.Replace("{username}", user.Username)
            .Replace("{time}", $"{TimeHelper.GetUnixDifference(user.Time)} ago")
            .Replace("{message}", user.MessageText?.Decode());
        ComingBack = string.IsNullOrEmpty(user.MessageText?.Decode()) ? ComingBack.Remove(":").ReplaceSpaces() : ComingBack;

        GoingAway = GoingAway.Replace("{username}", user.Username);

        Resuming = Resuming.Replace("{username}", user.Username);
    }
}
