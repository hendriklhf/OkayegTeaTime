using System.Reflection;
using HLE.Strings;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Twitch.Commands.AfkCommandClasses;
using OkayegTeaTimeCSharp.Twitch.Commands.Enums;
using OkayegTeaTimeCSharp.Twitch.Handlers;
using OkayegTeaTimeCSharp.Twitch.Messages.Interfaces;

namespace OkayegTeaTimeCSharp.Twitch.Commands;

public class CommandHandler : Handler
{
    private const string _handleName = "Handle";

    public CommandHandler(TwitchBot twitchBot)
        : base(twitchBot)
    {
    }

    public override void Handle(ITwitchChatMessage chatMessage)
    {
        if (chatMessage.IsCommand)
        {
            HandleCommand(chatMessage);
            TwitchBot.CommandCount++;
        }
        else if (chatMessage.IsAfkCommmand)
        {
            HandleAfkCommand(chatMessage);
            TwitchBot.CommandCount++;
        }
    }

    private void HandleCommand(ITwitchChatMessage chatMessage)
    {
        foreach (var type in (CommandType[])Enum.GetValues(typeof(CommandType)))
        {
            if (!CommandList.MatchesAnyAlias(chatMessage, type))
                continue;
            if (BotActions.IsOnCooldown(chatMessage.Username, type))
                continue;

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var alias in CommandList[type].Alias)
            {
                // ReSharper disable once InvertIf
                if (chatMessage.Message.IsMatch(PatternCreator.Create(alias, chatMessage.Channel.Prefix, @"(\s|$)")))
                {
                    BotActions.AddUserToCooldownDictionary(chatMessage.Username, type);
                    InvokeCommandHandle(type, TwitchBot, chatMessage, alias);
                    BotActions.AddCooldown(chatMessage.Username, type);
                    break;
                }
            }

            // Handled, we can jump out now
            break;
        }
    }

    private void HandleAfkCommand(ITwitchChatMessage chatMessage)
    {
        foreach (var type in (AfkCommandType[])Enum.GetValues(typeof(AfkCommandType)))
        {
            if (!CommandList.MatchesAnyAlias(chatMessage, type))
                continue;
            if (BotActions.IsOnAfkCooldown(chatMessage.Username))
                continue;

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var alias in CommandList[type].Alias)
            {
                // ReSharper disable once InvertIf
                if (chatMessage.Message.IsMatch(PatternCreator.Create(alias, chatMessage.Channel.Prefix, @"(\s|$)")))
                {
                    BotActions.AddUserToAfkCooldownDictionary(chatMessage.Username);
                    AfkCommandHandler.Handle(TwitchBot, chatMessage, type);
                    BotActions.AddAfkCooldown(chatMessage.Username);
                    break;
                }
            }

            break;
        }
    }

    private void InvokeCommandHandle(CommandType type, TwitchBot twitchBot, ITwitchChatMessage chatMessage, string alias)
    {
        Type commandClass = Type.GetType(CommandList.GetCommandClassName(type));
        ConstructorInfo constructor = commandClass.GetConstructor(new Type[] { typeof(TwitchBot), typeof(ITwitchChatMessage), typeof(string) });
        commandClass.GetMethod(_handleName).Invoke(constructor.Invoke(new object[] { twitchBot, chatMessage, alias }), null);
    }
}
