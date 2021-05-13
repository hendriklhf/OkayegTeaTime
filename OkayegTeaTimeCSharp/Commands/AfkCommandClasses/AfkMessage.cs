using OkayegTeaTimeCSharp.Database.Models;
using OkayegTeaTimeCSharp.Time;
using OkayegTeaTimeCSharp.Utils;

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
            return new AfkMessage(user.Type, CommandHelper.GetAfkCommand(user.Type).ComingBack, CommandHelper.GetAfkCommand(user.Type).GoingAway, CommandHelper.GetAfkCommand(user.Type).Resuming).ReplaceSpaceHolder(user);
        }

        public AfkMessage ReplaceSpaceHolder(User user)
        {
            ComingBack = ComingBack.Replace("{username}", user.Username)
                .Replace("{time}", TimeHelper.ConvertMillisecondsToPassedTime(user.Time, " ago"))
                .Replace("{message}", user.MessageText.Decode());

            GoingAway = ComingBack.Replace("{username}", user.Username);

            Resuming = Resuming.Replace("{username}", user.Username);

            return this;
        }
    }
}
