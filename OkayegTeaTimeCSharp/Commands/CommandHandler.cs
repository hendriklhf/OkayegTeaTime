using OkayegTeaTimeCSharp.Twitch.Bot;
using TwitchLib.Client.Models;
using OkayegTeaTimeCSharp.Commands.CommandEnums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OkayegTeaTimeCSharp.Commands
{
    public static class CommandHandler
    {
        public static void Handle(TwitchBot twitchBot, ChatMessage chatMessage)
        {
            ((CommandType[])Enum.GetValues(typeof(CommandType))).ToList().ForEach(type =>
            {
                if (CommandHelper.MatchesAlias(chatMessage, type))
                {
                    Type.GetType("Class").GetMethod("Handle").Invoke(null, null);
#warning unfinished
                }
            });
        }
    }
}
