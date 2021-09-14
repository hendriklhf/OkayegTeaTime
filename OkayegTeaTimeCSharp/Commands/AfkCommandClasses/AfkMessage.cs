using HLE.Strings;
using HLE.Time;
using HLE.Time.Enums;
using OkayegTeaTimeCSharp.Database.Models;
using OkayegTeaTimeCSharp.JsonData.JsonClasses.CommandData;

namespace OkayegTeaTimeCSharp.Commands.AfkCommandClasses
{
    public class AfkMessage
    {
        public string Name { get; }

        public string ComingBack { get; private set; }

        public string GoingAway { get; private set; }

        public string Resuming { get; private set; }

        private AfkMessage(string name, string comingBack, string goingAway, string resume)
        {
            Name = name;
            ComingBack = comingBack;
            GoingAway = goingAway;
            Resuming = resume;
        }

        public static AfkMessage Create(User user)
        {
            string type = user.Type.ToLower();
            AfkCommand afkCommand = CommandHelper.GetAfkCommand(type);
            return new AfkMessage(type, afkCommand.ComingBack, afkCommand.GoingAway, afkCommand.Resuming).ReplaceSpaceHolder(user);
        }

        private AfkMessage ReplaceSpaceHolder(User user)
        {
            ComingBack = ComingBack.Replace("{username}", user.Username)
                .Replace("{time}", TimeHelper.ConvertUnixTimeToTimeStamp(user.Time, "ago", ConversionType.YearDayHourMin))
                .Replace("{message}", user.MessageText?.Decode());
            ComingBack = string.IsNullOrEmpty(user.MessageText?.Decode()) ? ComingBack.Remove(":").ReplaceSpaces() : ComingBack;

            GoingAway = GoingAway.Replace("{username}", user.Username);

            Resuming = Resuming.Replace("{username}", user.Username);

            return this;
        }
    }
}
