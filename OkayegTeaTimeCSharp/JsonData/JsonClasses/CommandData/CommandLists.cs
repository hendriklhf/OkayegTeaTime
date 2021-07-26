using System.Collections.Generic;

namespace OkayegTeaTimeCSharp.JsonData.JsonClasses.CommandData
{
    public class CommandLists
    {
        public List<Command> Commands { get; set; }

        public List<AfkCommand> AfkCommands { get; set; }
    }
}
