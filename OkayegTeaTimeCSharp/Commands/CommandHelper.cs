using OkayegTeaTimeCSharp.Commands.CommandEnums;
using OkayegTeaTimeCSharp.JsonData;
using OkayegTeaTimeCSharp.JsonData.JsonClasses.CommandData;
using OkayegTeaTimeCSharp.Twitch;
using OkayegTeaTimeCSharp.Twitch.Bot;
using System.Collections.Generic;
using System.Linq;
using TwitchLib.Client.Models;
using JCommand = OkayegTeaTimeCSharp.JsonData.JsonClasses.CommandData.Command;

namespace OkayegTeaTimeCSharp.Commands
{
    public static class CommandHelper
    {
        public static AfkCommand GetAfkCommand(AfkCommandType type)
        {
            return JsonController.CommandLists.AfkCommands.FirstOrDefault(cmd => cmd.CommandName == type.ToString().ToLower());
        }

        public static AfkCommand GetAfkCommand(string name)
        {
            return JsonController.CommandLists.AfkCommands.FirstOrDefault(cmd => cmd.CommandName == name);
        }

        public static List<string> GetAfkCommandAliases()
        {
            List<string> listAlias = new();
            JsonController.CommandLists.AfkCommands.ForEach(cmd => cmd.Alias.ForEach(alias => listAlias.Add(alias)));
            return listAlias;
        }

        public static List<string> GetAllAliases()
        {
            return GetCommandAliases().Concat(GetAfkCommandAliases()).ToList();
        }

        public static JCommand GetCommand(CommandType type)
        {
            return JsonController.CommandLists.Commands.FirstOrDefault(cmd => cmd.CommandName == type.ToString().ToLower());
        }

        public static List<string> GetCommandAliases()
        {
            List<string> listAlias = new();
            JsonController.CommandLists.Commands.ForEach(cmd => cmd.Alias.ForEach(alias => listAlias.Add(alias)));
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

        public static bool MatchesAnyAlias(this ChatMessage chatMessage, CommandType type)
        {
            return GetCommand(type).Alias.Any(alias => PrefixHelper.GetPrefix(chatMessage.Channel) + alias == chatMessage.GetLowerSplit()[0] || alias + Config.Suffix == chatMessage.GetLowerSplit()[0]);
        }

        public static bool MatchesAnyAlias(this ChatMessage chatMessage, AfkCommandType type)
        {
            return GetAfkCommand(type).Alias.Any(alias => PrefixHelper.GetPrefix(chatMessage.Channel) + alias == chatMessage.GetLowerSplit()[0] || alias + Config.Suffix == chatMessage.GetLowerSplit()[0]);
        }
    }
}