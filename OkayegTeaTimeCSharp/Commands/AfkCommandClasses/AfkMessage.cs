using OkayegTeaTimeCSharp.Database.Models;

namespace OkayegTeaTimeCSharp.Commands.AfkCommandClasses
{
    public class AfkMessage
    {
        public string Name { get; private set; }

        public string ComingBack { get; set; }

        public string GoingAway { get; set; }

        private AfkMessage(string name, string comingBack, string goingAway)
        {
            Name = name;
            ComingBack = comingBack;
            GoingAway = goingAway;
        }

        public static AfkMessage Create(User user)
        {
            return new AfkMessage(user.Type, CommandHelper.GetAfkCommand(user.Type).ComingBack, CommandHelper.GetAfkCommand(user.Type).GoingAway).ReplaceSpaceHolder(user);
        }
    }
}
