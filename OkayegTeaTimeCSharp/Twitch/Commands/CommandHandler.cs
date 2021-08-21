using HLE.Collections;
using HLE.Strings;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Twitch.Commands.AfkCommandClasses;
using OkayegTeaTimeCSharp.Twitch.Commands.CommandEnums;
using OkayegTeaTimeCSharp.Twitch.Messages;
using OkayegTeaTimeCSharp.Utils;
using System;
using System.Reflection;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Twitch.Commands
{
    public class CommandHandler : Handler
    {
        public ChatMessage ChatMessage { get; }

        private const string _handleName = "Handle";

        public CommandHandler(TwitchBot twitchBot, TwitchLibMessage twitchLibMessage)
            : base(twitchBot)
        {
            ChatMessage = twitchLibMessage as ChatMessage;
        }

        public override void Handle()
        {
            if (ChatMessage.IsCommand())
            {
                ((CommandType[])Enum.GetValues(typeof(CommandType))).ForEach(type =>
                {
                    if (ChatMessage.MatchesAnyAlias(type))
                    {
                        if (!BotActions.IsOnCooldown(ChatMessage.Username, type))
                        {
                            CommandHelper.GetCommand(type).Alias.ForEach(alias =>
                            {
                                if (ChatMessage.GetMessage().IsMatch(PatternCreator.Create(alias, PrefixHelper.GetPrefix(ChatMessage.Channel), @"(\s|$)")))
                                {
                                    BotActions.AddUserToCooldownDictionary(ChatMessage.Username, type);
                                    InvokeCommandHandle(type, TwitchBot, ChatMessage, alias);
                                    BotActions.AddCooldown(ChatMessage.Username, type);
                                }
                            });
                        }
                    }
                });
                TwitchBot.CommandCount++;
            }
            else if (ChatMessage.IsAfkCommand())
            {
                ((AfkCommandType[])Enum.GetValues(typeof(AfkCommandType))).ForEach(type =>
                {
                    if (ChatMessage.MatchesAnyAlias(type))
                    {
                        if (!BotActions.IsOnAfkCooldown(ChatMessage.Username))
                        {
                            CommandHelper.GetAfkCommand(type).Alias.ForEach(alias =>
                            {
                                if (ChatMessage.GetMessage().IsMatch(PatternCreator.Create(alias, PrefixHelper.GetPrefix(ChatMessage.Channel), @"(\s|$)")))
                                {
                                    BotActions.AddUserToAfkCooldownDictionary(ChatMessage.Username);
                                    AfkCommandHandler.Handle(TwitchBot, ChatMessage, type);
                                    BotActions.AddAfkCooldown(ChatMessage.Username);
                                }
                            });
                        }
                    }
                });
                TwitchBot.CommandCount++;
            }
        }

        private static void InvokeCommandHandle(CommandType type, TwitchBot twitchBot, ChatMessage chatMessage, string alias)
        {
            Type commandClass = Type.GetType(CommandHelper.GetCommandClassName(type));
            ConstructorInfo constructor = commandClass.GetConstructor(new Type[] { typeof(TwitchBot), typeof(ChatMessage), typeof(string) });
            commandClass.GetMethod(_handleName).Invoke(constructor.Invoke(new object[] { twitchBot, chatMessage, alias }), null);
        }
    }
}
