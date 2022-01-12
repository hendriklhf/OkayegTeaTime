using System.Reflection;
using System.Text.RegularExpressions;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Bot.Cooldowns;
using OkayegTeaTime.Twitch.Commands.AfkCommandClasses;
using OkayegTeaTime.Twitch.Commands.Enums;
using OkayegTeaTime.Twitch.Handlers;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

public class CommandHandler : Handler
{
    private const string _handleName = "Handle";
    private const string _sendResponseName = "SendResponse";

    public CommandHandler(TwitchBot twitchBot)
        : base(twitchBot)
    {
    }

    public override void Handle(TwitchChatMessage chatMessage)
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

    private void HandleCommand(TwitchChatMessage chatMessage)
    {
        bool handled = false;
        foreach (CommandType type in (CommandType[])Enum.GetValues(typeof(CommandType)))
        {
            if (handled)
            {
                break;
            }

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (string alias in AppSettings.CommandList[type].Alias)
            {
                Regex pattern = PatternCreator.Create(alias, chatMessage.Channel.Prefix, @"(\s|$)");

                // ReSharper disable once InvertIf
                if (pattern.IsMatch(chatMessage.Message))
                {
                    if (CooldownController.IsOnCooldown(chatMessage.UserId, type))
                    {
                        handled = true;
                        break;
                    }

                    InvokeCommandHandle(type, TwitchBot, chatMessage, alias);
                    CooldownController.AddCooldown(chatMessage.UserId, type);
                    handled = true;
                    break;
                }
            }
        }
    }

    private void HandleAfkCommand(TwitchChatMessage chatMessage)
    {
        bool handled = false;
        foreach (AfkCommandType type in (AfkCommandType[])Enum.GetValues(typeof(AfkCommandType)))
        {
            if (handled)
            {
                break;
            }

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (string alias in AppSettings.CommandList[type].Alias)
            {
                Regex pattern = PatternCreator.Create(alias, chatMessage.Channel.Prefix, @"(\s|$)");

                // ReSharper disable once InvertIf
                if (pattern.IsMatch(chatMessage.Message))
                {
                    if (CooldownController.IsOnAfkCooldown(chatMessage.UserId))
                    {
                        handled = true;
                        break;
                    }

                    AfkCommandHandler.Handle(TwitchBot, chatMessage, type);
                    CooldownController.AddAfkCooldown(chatMessage.UserId);
                    handled = true;
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Attempts to handle a command through a handler via reflection
    /// </summary>
    /// <param name="type">The command handler class type</param>
    /// <param name="twitchBot">The currently running bot that received this command</param>
    /// <param name="chatMessage">The chat message to handle</param>
    /// <param name="alias">A command alias</param>
    /// <exception cref="InvalidOperationException">The command handler doesn't conform</exception>
    private static void InvokeCommandHandle(CommandType type, TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
    {
        string? commandClassName = AppSettings.CommandList.GetCommandClassName(type);

        Type? commandClass = Type.GetType(commandClassName);
        if (commandClass is null)
        {
            throw new InvalidOperationException($"Could not get type of command class {commandClassName}");
        }

        ConstructorInfo? constructor = commandClass.GetConstructor(new[] { typeof(TwitchBot), typeof(TwitchChatMessage), typeof(string) });
        if (constructor is null)
        {
            throw new InvalidOperationException($"Could not instantiate command class {commandClassName}");
        }

        object? handlerInstance = constructor.Invoke(new object[] { twitchBot, chatMessage, alias });

        MethodInfo? handleMethod = commandClass.GetMethod(_handleName);
        if (handleMethod is null)
        {
            throw new InvalidOperationException($"Could not get handler method for command class {commandClassName}");
        }
        handleMethod.Invoke(handlerInstance, null);

        MethodInfo? sendMethod = commandClass.GetMethod(_sendResponseName);
        if (sendMethod is null)
        {
            throw new InvalidOperationException($"Could not get send method for command class {commandClassName}");
        }
        sendMethod.Invoke(handlerInstance, null);
    }
}
