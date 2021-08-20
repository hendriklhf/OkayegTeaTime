using HLE.Collections;
using HLE.Strings;
using OkayegTeaTimeCSharp.Commands.AfkCommandClasses;
using OkayegTeaTimeCSharp.Commands.CommandEnums;
using OkayegTeaTimeCSharp.Messages;
using OkayegTeaTimeCSharp.Twitch;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;
using System;
using System.Reflection;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Commands
{
    public static class CommandHandler
    {
        private const string _handleName = "Handle";

        public static void Handle(TwitchBot twitchBot, ChatMessage chatMessage)
        {
            if (chatMessage.IsCommand())
            {
                ((CommandType[])Enum.GetValues(typeof(CommandType))).ForEach(type =>
                {
                    if (chatMessage.MatchesAnyAlias(type))
                    {
                        if (!BotActions.IsOnCooldown(chatMessage.Username, type))
                        {
                            CommandHelper.GetCommand(type).Alias.ForEach(alias =>
                            {
                                if (chatMessage.GetMessage().IsMatch(PatternCreator.Create(alias, PrefixHelper.GetPrefix(chatMessage.Channel), @"(\s|$)")))
                                {
                                    BotActions.AddUserToCooldownDictionary(chatMessage.Username, type);
                                    Type commandClass = Type.GetType(CommandHelper.GetCommandClassName(type));
                                    ConstructorInfo constructor = commandClass.GetConstructor(new Type[] { typeof(TwitchBot), typeof(ChatMessage), typeof(string) });
                                    commandClass.GetMethod(_handleName).Invoke(constructor.Invoke(new object[] { twitchBot, chatMessage, alias }), null);
                                    BotActions.AddCooldown(chatMessage.Username, type);
                                }
                            });
                        }
                    }
                });
                twitchBot.CommandCount++;
            }
            else if (chatMessage.IsAfkCommand())
            {
                ((AfkCommandType[])Enum.GetValues(typeof(AfkCommandType))).ForEach(type =>
                {
                    if (chatMessage.MatchesAnyAlias(type))
                    {
                        if (!BotActions.IsOnAfkCooldown(chatMessage.Username))
                        {
                            CommandHelper.GetAfkCommand(type).Alias.ForEach(alias =>
                            {
                                if (chatMessage.GetMessage().IsMatch(PatternCreator.Create(alias, PrefixHelper.GetPrefix(chatMessage.Channel), @"(\s|$)")))
                                {
                                    BotActions.AddUserToAfkCooldownDictionary(chatMessage.Username);
                                    AfkCommandHandler.Handle(twitchBot, chatMessage, type);
                                    BotActions.AddAfkCooldown(chatMessage.Username);
                                }
                            });
                        }
                    }
                });
                twitchBot.CommandCount++;
            }
        }
    }
}