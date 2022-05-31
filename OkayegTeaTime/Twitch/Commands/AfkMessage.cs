using HLE.Strings;
using HLE.Time;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Files.Jsons.CommandData;

namespace OkayegTeaTime.Twitch.Commands;

public class AfkMessage
{
    public User? User { get; }

    public string? ComingBack { get; private set; }

    public string? GoingAway { get; private set; }

    public string? Resuming { get; private set; }

    public AfkMessage(long userId, AfkCommand cmd)
    {
        User = DbControl.Users[userId];
        CreateMessages(cmd);
    }

    private void CreateMessages(AfkCommand cmd)
    {
        if (User is null)
        {
            return;
        }

        ComingBack = cmd.ComingBack.Replace("{username}", User.Username)
            .Replace("{time}", $"{TimeHelper.GetUnixDifference(User.AfkTime)} ago")
            .Replace("{message}", User.AfkMessage);
        ComingBack = string.IsNullOrEmpty(User.AfkMessage) ? ComingBack.Remove(":").TrimAll() : ComingBack;

        GoingAway = cmd.GoingAway.Replace("{username}", User.Username);

        Resuming = cmd.Resuming.Replace("{username}", User.Username);
    }
}
