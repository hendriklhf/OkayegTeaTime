using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkayegTeaTimeCSharp.JsonData.JsonClasses.CommandData
{
    public class CommandLists
    {
        public List<Command> Commands { get; set; }

        public List<AfkCommand> AfkCommands { get; set; }
    }
}
