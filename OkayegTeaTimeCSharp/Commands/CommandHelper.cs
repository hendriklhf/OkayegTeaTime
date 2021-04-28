using OkayegTeaTimeCSharp.JsonData;
using OkayegTeaTimeCSharp.JsonData.JsonClasses;
using System.Linq;

namespace OkayegTeaTimeCSharp.Commands
{
    public static class CommandHelper
    {
        public static Command GetCommand(string name)
        {
            return JsonHelper.JsonToObject().CommandLists.Commands.Where(cmd => cmd.Alias.Any(alias => alias == name.ToLower())).FirstOrDefault();
        }

        public static AfkCommand GetAfkCommand(string name)
        {
            return JsonHelper.JsonToObject().CommandLists.AfkCommands.Where(cmd => cmd.Alias.Any(alias => alias == name.ToLower())).FirstOrDefault();
        }
    }
}
