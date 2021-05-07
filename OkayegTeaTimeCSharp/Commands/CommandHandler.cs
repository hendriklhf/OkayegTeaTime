using OkayegTeaTimeCSharp.Commands.CommandEnums;
using OkayegTeaTimeCSharp.Messages;
using OkayegTeaTimeCSharp.Time;
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
        private static readonly string _handleName = "Handle";

        public static void Handle(TwitchBot twitchBot, ChatMessage chatMessage)
        {
            if (MessageHelper.IsCommand(chatMessage.GetMessage()))
            {
                ((CommandType[])Enum.GetValues(typeof(CommandType))).ToList().ForEach(type =>
                {
                    if (CommandHelper.MatchesAlias(chatMessage, type))
                    {
                        if (!BotHelper.IsOnCooldown(chatMessage.Username, type))
                        {
                            if (string.IsNullOrEmpty(PrefixHelper.GetPrefix(chatMessage.Channel)))
                            {
                                CommandHelper.GetCommand(type).Alias.ForEach(alias =>
                                {
                                    if (chatMessage.GetMessage().IsMatch(PatternCreator.Create(alias, PrefixType.None)))
                                    {
                                        BotHelper.AddUserToCooldownDictionary(chatMessage.UserId, type, TimeHelper.Now());
                                        Type.GetType(CommandHelper.GetCommandClassName(type)).GetMethod(_handleName).Invoke(null, new object[] { twitchBot, chatMessage, alias });
                                        BotHelper.AddCooldown(chatMessage.Username, type);
                                    }
                                });
                            }
                            else
                            {
                                CommandHelper.GetCommand(type).Alias.ForEach(alias =>
                                {
                                    if (chatMessage.GetMessage().IsMatch(PatternCreator.Create(alias, PrefixType.Active)))
                                    {
                                        BotHelper.AddUserToCooldownDictionary(chatMessage.UserId, type, TimeHelper.Now());
                                        Type.GetType(CommandHelper.GetCommandClassName(type)).GetMethod(_handleName).Invoke(null, new object[] { twitchBot, chatMessage, alias });
                                        BotHelper.AddCooldown(chatMessage.Username, type);
                                    }
                                });
                            }
                        }
                    }
                });
            }
            else if (MessageHelper.IsAfkCommand(chatMessage.GetMessage()))
            {
#warning not implemented
            }
        }
    }
}
