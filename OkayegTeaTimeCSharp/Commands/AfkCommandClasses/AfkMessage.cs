using OkayegTeaTimeCSharp.Database.Models;

namespace OkayegTeaTimeCSharp.Commands.AfkCommandClasses
{
    public class AfkMessage
    {
        public string Name { get; }

        public string ComingBack { get; }

        public string GoingAway { get; }

        public string Resuming { get; }

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
    }
}
