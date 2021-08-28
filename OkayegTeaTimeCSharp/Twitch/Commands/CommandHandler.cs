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

        public CommandHandler(TwitchBot twitchBot, ChatMessage chatMessage)
            : base(twitchBot)
        {
            ChatMessage = chatMessage;
        }

        public override void Handle()
        {
            if (ChatMessage.IsCommand())
            {
                foreach (CommandType type in (CommandType[])Enum.GetValues(typeof(CommandType)))
                {
                    if (ChatMessage.MatchesAnyAlias(type))
                    {
                        if (!BotActions.IsOnCooldown(ChatMessage.Username, type))
                        {
                            foreach (string alias in CommandHelper.GetCommand(type).Alias)
                            {
                                if (ChatMessage.GetMessage().IsMatch(PatternCreator.Create(alias, PrefixHelper.GetPrefix(ChatMessage.Channel), @"(\s|$)")))
                                {
                                    BotActions.AddUserToCooldownDictionary(ChatMessage.Username, type);
                                    InvokeCommandHandle(type, TwitchBot, ChatMessage, alias);
                                    BotActions.AddCooldown(ChatMessage.Username, type);
                                    break;
                                }
                            }
                            break;
                        }
                    }
                }
                TwitchBot.CommandCount++;
            }
            else if (ChatMessage.IsAfkCommand())
            {
                foreach (AfkCommandType type in (AfkCommandType[])Enum.GetValues(typeof(AfkCommandType)))
                {
                    if (ChatMessage.MatchesAnyAlias(type))
                    {
                        if (!BotActions.IsOnAfkCooldown(ChatMessage.Username))
                        {
                            foreach (string alias in CommandHelper.GetAfkCommand(type).Alias)
                            {
                                if (ChatMessage.GetMessage().IsMatch(PatternCreator.Create(alias, PrefixHelper.GetPrefix(ChatMessage.Channel), @"(\s|$)")))
                                {
                                    BotActions.AddUserToAfkCooldownDictionary(ChatMessage.Username);
                                    AfkCommandHandler.Handle(TwitchBot, ChatMessage, type);
                                    BotActions.AddAfkCooldown(ChatMessage.Username);
                                    break;
                                }
                            }
                            break;
                        }
                    }
                }
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
