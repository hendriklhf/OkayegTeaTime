using OkayegTeaTimeCSharp.Commands.Enums;
using OkayegTeaTimeCSharp.JsonData;
using OkayegTeaTimeCSharp.JsonData.JsonClasses.CommandData;
using OkayegTeaTimeCSharp.Messages.Interfaces;
using OkayegTeaTimeCSharp.Twitch;
using OkayegTeaTimeCSharp.Twitch.Bot;
using JCommand = OkayegTeaTimeCSharp.JsonData.JsonClasses.CommandData.Command;

namespace OkayegTeaTimeCSharp.Commands
{
    public static class CommandHelper
    {
        public static AfkCommand GetAfkCommand(AfkCommandType type)
        {
            return new JsonController().CommandLists.AfkCommands.FirstOrDefault(cmd => cmd.CommandName == type.ToString().ToLower());
        }

        public static AfkCommand GetAfkCommand(string name)
        {
            return new JsonController().CommandLists.AfkCommands.FirstOrDefault(cmd => cmd.CommandName == name);
        }

        public static List<string> GetAfkCommandAliases()
        {
            List<string> listAlias = new();
            new JsonController().CommandLists.AfkCommands.ForEach(cmd => cmd.Alias.ForEach(alias => listAlias.Add(alias)));
            return listAlias;
        }

        public static List<string> GetAllAliases()
        {
            return GetCommandAliases().Concat(GetAfkCommandAliases()).ToList();
        }

        public static JCommand GetCommand(CommandType type)
        {
            return new JsonController().CommandLists.Commands.FirstOrDefault(cmd => cmd.CommandName == type.ToString().ToLower());
        }

        public static List<string> GetCommandAliases()
        {
            List<string> listAlias = new();
            new JsonController().CommandLists.Commands.ForEach(cmd => cmd.Alias.ForEach(alias => listAlias.Add(alias)));
            return listAlias;
        }

        public static string GetCommandClassName(CommandType type)
        {
            return $"OkayegTeaTimeCSharp.Commands.CommandClasses.{type}Command";
        }

        public static long GetCoolDown(CommandType type)
        {
            return GetCommand(type).Cooldown;
        }

        public static bool MatchesAnyAlias(this ITwitchChatMessage chatMessage, CommandType type)
        {
            return GetCommand(type).Alias.Any(alias => PrefixDictionary.Get(chatMessage.Channel) + alias == chatMessage.LowerSplit[0] || alias + TwitchConfig.Suffix == chatMessage.LowerSplit[0]);
        }

        public static bool MatchesAnyAlias(this ITwitchChatMessage chatMessage, AfkCommandType type)
        {
            return GetAfkCommand(type).Alias.Any(alias => PrefixDictionary.Get(chatMessage.Channel) + alias == chatMessage.LowerSplit[0] || alias + TwitchConfig.Suffix == chatMessage.LowerSplit[0]);
        }
    }
}
