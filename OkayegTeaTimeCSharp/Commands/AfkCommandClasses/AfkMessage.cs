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
            string type = user.Type.ToLower();
            return new AfkMessage(type, CommandHelper.GetAfkCommand(type).ComingBack, CommandHelper.GetAfkCommand(type).GoingAway, CommandHelper.GetAfkCommand(type).Resuming).ReplaceSpaceHolder(user);
        }

        private AfkMessage ReplaceSpaceHolder(User user)
        {
            ComingBack = ComingBack.Replace("{username}", user.Username)
                .Replace("{time}", TimeHelper.ConvertMillisecondsToPassedTime(user.Time, "ago"))
                .Replace("{message}", user.MessageText.Decode());

            GoingAway = GoingAway.Replace("{username}", user.Username);

            Resuming = Resuming.Replace("{username}", user.Username);

            return this;
        }
    }
}