using System.Reflection;
using OkayegTeaTime.Database;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Commands;
using OkayegTeaTime.Twitch.Commands.Enums;
using OkayegTeaTime.Twitch.Controller;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Handlers;

public class CommandHandler : Handler
{
    private readonly AfkCommandHandler _afkCommandHandler;
    private readonly CooldownController _cooldownController;

    private readonly CommandType[] _commandTypes = Enum.GetValues<CommandType>();
    private readonly AfkCommandType[] _afkCommandTypes = Enum.GetValues<AfkCommandType>();

    public CommandHandler(TwitchBot twitchBot) : base(twitchBot)
    {
        _afkCommandHandler = new(twitchBot);
        _cooldownController = new(twitchBot);
    }

    public override void Handle(TwitchChatMessage chatMessage)
    {
        bool handled = HandleCommand(chatMessage);
        if (!handled)
        {
            HandleAfkCommand(chatMessage);
        }
    }

    private bool HandleCommand(TwitchChatMessage chatMessage)
    {
        foreach (CommandType type in _commandTypes)
        {
            foreach (string alias in _twitchBot.CommandController[type].Alias)
            {
                string? prefix = DbControl.Channels[chatMessage.ChannelId]?.Prefix;
                Regex pattern = PatternCreator.Create(alias, prefix);

                if (pattern.IsMatch(chatMessage.Message))
                {
                    if (_cooldownController.IsOnCooldown(chatMessage.UserId, type))
                    {
                        return false;
                    }

                    InvokeCommandHandle(type, _twitchBot, chatMessage, alias);
                    _cooldownController.AddCooldown(chatMessage.UserId, type);
                    _twitchBot.CommandCount++;
                    return true;
                }
            }
        }

        return false;
    }

    private void HandleAfkCommand(TwitchChatMessage chatMessage)
    {
        foreach (AfkCommandType type in _afkCommandTypes)
        {
            foreach (string alias in _twitchBot.CommandController[type].Alias)
            {
                string? prefix = DbControl.Channels[chatMessage.ChannelId]?.Prefix;
                Regex pattern = PatternCreator.Create(alias, prefix);

                if (pattern.IsMatch(chatMessage.Message))
                {
                    if (_cooldownController.IsOnAfkCooldown(chatMessage.UserId))
                    {
                        return;
                    }

                    _afkCommandHandler.Handle(chatMessage, type);
                    _cooldownController.AddAfkCooldown(chatMessage.UserId);
                    _twitchBot.CommandCount++;
                    return;
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
        string commandClassName = $"{AppSettings.AssemblyName}.Twitch.Commands.{type}Command";

        Type? commandClass = Type.GetType(commandClassName);
        if (commandClass is null)
        {
            throw new InvalidOperationException($"Could not get type of command class {commandClassName}");
        }

        ConstructorInfo? constructor = commandClass.GetConstructor(new[]
        {
            typeof(TwitchBot),
            typeof(TwitchChatMessage),
            typeof(string)
        });
        if (constructor is null)
        {
            throw new InvalidOperationException($"Could not instantiate command class {commandClassName}");
        }

        object handlerInstance = constructor.Invoke(new object[]
        {
            twitchBot,
            chatMessage,
            alias
        });

        MethodInfo? handleMethod = commandClass.GetMethod(nameof(Command.Handle));
        if (handleMethod is null)
        {
            throw new InvalidOperationException($"Could not get handler method for command class {commandClassName}");
        }

        handleMethod.Invoke(handlerInstance, null);

        MethodInfo? sendMethod = commandClass.GetMethod(nameof(Command.SendResponse));
        if (sendMethod is null)
        {
            throw new InvalidOperationException($"Could not get send method for command class {commandClassName}");
        }

        sendMethod.Invoke(handlerInstance, null);
    }
}
