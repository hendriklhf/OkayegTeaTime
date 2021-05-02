using OkayegTeaTimeCSharp.Commands.CommandEnums;
using OkayegTeaTimeCSharp.Messages;
using OkayegTeaTimeCSharp.Prefixes;
using OkayegTeaTimeCSharp.Twitch;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;
using System;
using System.Linq;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Commands
{
    public static class CommandHandler
    {
        public static void Handle(TwitchBot twitchBot, ChatMessage chatMessage)
        {
            if (MessageHelper.IsCommand(chatMessage.GetMessage()))
            {
                ((CommandType[])Enum.GetValues(typeof(CommandType))).ToList().ForEach(type =>
                {
                    if (CommandHelper.MatchesAlias(chatMessage, type))
                    {
                        if (string.IsNullOrEmpty(PrefixHelper.GetPrefix(chatMessage.Channel)))
                        {
                            CommandHelper.GetCommand(type).Alias.ForEach(alias =>
                            {
                                if (chatMessage.GetMessage().IsMatch(PatternCreator.Create(alias, PrefixType.None)))
                                {
                                    Type.GetType(CommandHelper.GetCommandClassName(type)).GetMethod("Handle").Invoke(null, new object[] { twitchBot, chatMessage, alias, type });
                                }
                            });
                        }
                        else
                        {
                            CommandHelper.GetCommand(type).Alias.ForEach(alias =>
                            {
                                if (chatMessage.GetMessage().IsMatch(PatternCreator.Create(alias, PrefixType.Active)))
                                {
                                    Type.GetType(CommandHelper.GetCommandClassName(type)).GetMethod("Handle").Invoke(null, new object[] { twitchBot, chatMessage, alias, type });
                                }
                            });
                        }
                    }
                });
            }
            else if (MessageHelper.IsAfkCommand(chatMessage.GetMessage()))
            {

            }
        }
    }
}
