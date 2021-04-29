using OkayegTeaTimeCSharp.Commands.AfkCommands;
using OkayegTeaTimeCSharp.Database.Models;
using OkayegTeaTimeCSharp.JsonData;
using OkayegTeaTimeCSharp.JsonData.JsonClasses;
using OkayegTeaTimeCSharp.Time;
using System.Collections.Generic;
using System.Linq;

namespace OkayegTeaTimeCSharp.Commands
{
    public static class CommandHelper
    {
        public static Command GetCommand(string name)
        {
            return JsonHelper.BotData.CommandLists.Commands.Where(cmd => cmd.Alias.Any(alias => alias == name.ToLower())).FirstOrDefault();
        }

        public static AfkCommand GetAfkCommand(string name)
        {
            return JsonHelper.BotData.CommandLists.AfkCommands.Where(cmd => cmd.Alias.Any(alias => alias == name.ToLower())).FirstOrDefault();
        }

        public static List<string> GetCommandAliases()
        {
            List<string> listAlias = new();
            JsonHelper.BotData.CommandLists.Commands.ForEach(cmd =>
            {
                cmd.Alias.ForEach(alias =>
                {
                    listAlias.Add(alias);
                });
            });
            return listAlias;
        }

        public static List<string> GetAfkCommandAliases()
        {
            List<string> listAlias = new();
            JsonHelper.BotData.CommandLists.AfkCommands.ForEach(cmd =>
            {
                cmd.Alias.ForEach(alias =>
                {
                    listAlias.Add(alias);
                });
            });
            return listAlias;
        }

        public static List<string> GetAllAliases()
        {
            return GetCommandAliases().Concat(GetAfkCommandAliases()).ToList();
        }

        public static AfkMessage ReplaceSpaceHolder(this AfkMessage afkMessage, User user)
        {
            afkMessage.ComingBack = afkMessage.ComingBack.Replace("{username}", user.Username)
                .Replace("{time}", TimeHelper.ConvertMillisecondsToPassedTime(user.Time, " ago"))
                .Replace("{message}", user.MessageText.ToString());

            afkMessage.GoingAway = afkMessage.ComingBack.Replace("{username}", user.Username)
                .Replace("{time}", TimeHelper.ConvertMillisecondsToPassedTime(user.Time, " ago"))
                .Replace("{message}", user.MessageText.ToString());

            return afkMessage;
        }
    }
}
