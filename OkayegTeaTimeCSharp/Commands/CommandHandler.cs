using OkayegTeaTimeCSharp.Commands.CommandEnums;
using OkayegTeaTimeCSharp.Messages;
using OkayegTeaTimeCSharp.Twitch;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;
using System;
using System.Linq;
using TwitchLib.Client.Models;
using OkayegTeaTimeCSharp.Commands.AfkCommandClasses;

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
                    if (chatMessage.MatchesAnyAlias(type))
                    {
                        if (!BotActions.IsOnCooldown(chatMessage.Username, type))
                        {
                            if (string.IsNullOrEmpty(PrefixHelper.GetPrefix(chatMessage.Channel)))
                            {
                                CommandHelper.GetCommand(type).Alias.ForEach(alias =>
                                {
                                    if (chatMessage.GetMessage().IsMatch(PatternCreator.Create(alias, PrefixType.None)))
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
                                    if (chatMessage.GetMessage().IsMatch(PatternCreator.Create(alias, PrefixType.Active)))
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
            }
            else if (MessageHelper.IsAfkCommand(chatMessage.GetMessage()))
            {
                if (MessageHelper.IsAfkCommand(chatMessage.GetMessage()))
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
                                        if (chatMessage.GetMessage().IsMatch(PatternCreator.Create(alias, PrefixType.None)))
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
                                        if (chatMessage.GetMessage().IsMatch(PatternCreator.Create(alias, PrefixType.Active)))
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
                }
            }
        }
    }
}