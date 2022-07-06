using HLE;
using HLE.Time;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Files.Jsons.CommandData;

namespace OkayegTeaTime.Twitch.Commands;

public class AfkMessage
{
    public string? ComingBack { get; private set; }

    public string? GoingAway { get; private set; }

    public string? Resuming { get; private set; }

    public AfkMessage(User user, AfkCommand cmd)
    {
        ComingBack = cmd.ComingBack.Replace("{username}", user.Username)
            .Replace("{time}", $"{TimeHelper.GetUnixDifference(user.AfkTime)} ago")
            .Replace("{message}", user.AfkMessage);
        ComingBack = string.IsNullOrEmpty(user.AfkMessage) ? ComingBack.Remove(":").TrimAll() : ComingBack;

        GoingAway = cmd.GoingAway.Replace("{username}", user.Username);

        Resuming = cmd.Resuming.Replace("{username}", user.Username);
    }
}
