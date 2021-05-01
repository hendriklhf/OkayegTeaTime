using OkayegTeaTimeCSharp.Commands.CommandEnums;
using OkayegTeaTimeCSharp.Twitch.Bot;
using System;
using System.Linq;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Commands
{
    public static class CommandHandler
    {
        public static void Handle(TwitchBot twitchBot, ChatMessage chatMessage)
        {
#warning not sure if this is the best method to do it
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
