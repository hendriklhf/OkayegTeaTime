namespace OkayegTeaTimeCSharp.Commands.AfkCommands
{
    public class AfkMessage
    {
        public string ComingBack { get; private set; }

        public string GoingAway { get; private set; }

        private AfkMessage(string comingBack, string goingAway)
        {
            ComingBack = comingBack;
            GoingAway = goingAway;
        }

#warning maybe username as parameter
        public static AfkMessage Create(string afkCommandName)
        {
            return new AfkMessage(CommandHelper.GetAfkCommand(afkCommandName).ComingBack, CommandHelper.GetAfkCommand(afkCommandName).GoingAway);
        }
    }
}
