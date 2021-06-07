using OkayegTeaTimeCSharp.Commands.AfkCommandClasses;
using OkayegTeaTimeCSharp.Commands.CommandEnums;
using OkayegTeaTimeCSharp.Messages;
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
            if (MessageHelper.IsCommand(chatMessage))
            {
                ((CommandType[])Enum.GetValues(typeof(CommandType))).ToList().ForEach(type =>
                {
                    if (chatMessage.MatchesAnyAlias(type))
                    {
                        if (!BotActions.IsOnCooldown(chatMessage.Username, type))
                        {
                            if (string.IsNullOrEmpty(PrefixHelper.GetPrefix(chatMessage.Channel)))
                            {
                                CommandHelper.GetCommand(type).Alias.ForEach(alias =>
                                {
                                    if (chatMessage.GetMessage().IsMatch(PatternCreator.Create(alias, PrefixType.None, PrefixHelper.GetPrefix(chatMessage.Channel))))
                                    {
                                        BotActions.AddUserToCooldownDictionary(chatMessage.Username, type);
                                        Type.GetType(CommandHelper.GetCommandClassName(type)).GetMethod(_handleName).Invoke(null, new object[] { twitchBot, chatMessage, alias });
                                        BotActions.AddCooldown(chatMessage.Username, type);
                                    }
                                });
                            }
                            else
                            {
                                CommandHelper.GetCommand(type).Alias.ForEach(alias =>
                                {
                                    if (chatMessage.GetMessage().IsMatch(PatternCreator.Create(alias, PrefixType.Active, PrefixHelper.GetPrefix(chatMessage.Channel))))
                                    {
                                        BotActions.AddUserToCooldownDictionary(chatMessage.Username, type);
                                        Type.GetType(CommandHelper.GetCommandClassName(type)).GetMethod(_handleName).Invoke(null, new object[] { twitchBot, chatMessage, alias });
                                        BotActions.AddCooldown(chatMessage.Username, type);
                                    }
                                });
                            }
                        }
                    }
                });
                twitchBot.CommandCount++;
            }
            else if (MessageHelper.IsAfkCommand(chatMessage))
            {
                ((AfkCommandType[])Enum.GetValues(typeof(AfkCommandType))).ToList().ForEach(type =>
                {
                    if (chatMessage.MatchesAnyAlias(type))
                    {
                        if (!BotActions.IsOnAfkCooldown(chatMessage.Username))
                        {
                            if (string.IsNullOrEmpty(PrefixHelper.GetPrefix(chatMessage.Channel)))
                            {
                                CommandHelper.GetAfkCommand(type).Alias.ForEach(alias =>
                                {
                                    if (chatMessage.GetMessage().IsMatch(PatternCreator.Create(alias, PrefixType.None, PrefixHelper.GetPrefix(chatMessage.Channel))))
                                    {
                                        BotActions.AddUserToAfkCooldownDictionary(chatMessage.Username);
                                        AfkCommandHandler.Handle(twitchBot, chatMessage, type);
                                        BotActions.AddAfkCooldown(chatMessage.Username);
                                    }
                                });
                            }
                            else
                            {
                                CommandHelper.GetAfkCommand(type).Alias.ForEach(alias =>
                                {
                                    if (chatMessage.GetMessage().IsMatch(PatternCreator.Create(alias, PrefixType.Active, PrefixHelper.GetPrefix(chatMessage.Channel))))
                                    {
                                        BotActions.AddUserToAfkCooldownDictionary(chatMessage.Username);
                                        AfkCommandHandler.Handle(twitchBot, chatMessage, type);
                                        BotActions.AddAfkCooldown(chatMessage.Username);
                                    }
                                });
                            }
                        }
                    }
                });
                twitchBot.CommandCount++;
            }
        }
    }
}