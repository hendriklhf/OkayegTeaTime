using HLE.Enums;
using HLE.Strings;
using HLE.Time;
using HLE.Time.Enums;
using OkayegTeaTimeCSharp.Database.Models;
using OkayegTeaTimeCSharp.Files;
using OkayegTeaTimeCSharp.Files.JsonClasses.CommandData;
using OkayegTeaTimeCSharp.Twitch.Commands.Enums;

namespace OkayegTeaTimeCSharp.Twitch.Commands.AfkCommandClasses;

public class AfkMessage
{
    public string ComingBack { get; private set; }

    public string GoingAway { get; private set; }

    public string Resuming { get; private set; }

    public AfkMessage(User user)
    {
        string type = user.Type.ToLower();
        List<AfkCommandType> afkTypes = typeof(AfkCommandType).ToList<AfkCommandType>();
        AfkCommand afkCommand = JsonController.CommandList[afkTypes.FirstOrDefault(t => t.ToString().ToLower() == type)];
        ComingBack = afkCommand.ComingBack;
        GoingAway = afkCommand.GoingAway;
        Resuming = afkCommand.Resuming;
        ReplaceSpaceHolder(user);
    }

    private void ReplaceSpaceHolder(User user)
    {
        ComingBack = ComingBack.Replace("{username}", user.Username)
            .Replace("{time}", TimeHelper.ConvertUnixTimeToTimeStamp(user.Time, "ago", ConversionType.YearDayHourMin))
            .Replace("{message}", user.MessageText?.Decode());
        ComingBack = string.IsNullOrEmpty(user.MessageText?.Decode()) ? ComingBack.Remove(":").ReplaceSpaces() : ComingBack;

        GoingAway = GoingAway.Replace("{username}", user.Username);

        Resuming = Resuming.Replace("{username}", user.Username);
    }
}
