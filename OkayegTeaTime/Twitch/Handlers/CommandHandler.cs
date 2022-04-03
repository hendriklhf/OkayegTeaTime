using System.Reflection;
using OkayegTeaTime.Database;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Commands.Enums;
using OkayegTeaTime.Twitch.Controller;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Handlers;

public class CommandHandler : Handler
{
    public AfkCommandHandler AfkCommandHandler { get; }

    public CooldownController CooldownController { get; }

    public CommandType[] CommandTypes { get; } = (CommandType[])Enum.GetValues(typeof(CommandType));

    public AfkCommandType[] AfkCommandTypes { get; } = (AfkCommandType[])Enum.GetValues(typeof(AfkCommandType));

    private bool _handled;

    private const string _handleName = "Handle";
    private const string _sendResponseName = "SendResponse";

    public CommandHandler(TwitchBot twitchBot)
        : base(twitchBot)
    {
        AfkCommandHandler = new(twitchBot);
        CooldownController = new();
    }

    public override void Handle(TwitchChatMessage chatMessage)
    {
        _handled = false;
        HandleCommand(chatMessage);
        if (!_handled)
        {
            HandleAfkCommand(chatMessage);
        }
    }

    private void HandleCommand(TwitchChatMessage chatMessage)
    {
        foreach (CommandType type in CommandTypes)
        {
            foreach (string alias in AppSettings.CommandList[type].Alias)
            {
                string? prefix = DbControl.Channels[chatMessage.ChannelId]?.Prefix;
                Regex pattern = PatternCreator.Create(alias, prefix);

                if (pattern.IsMatch(chatMessage.Message))
                {
                    _handled = true;

                    if (CooldownController.IsOnCooldown(chatMessage.UserId, type))
                    {
                        return;
                    }

                    InvokeCommandHandle(type, _twitchBot, chatMessage, alias);
                    CooldownController.AddCooldown(chatMessage.UserId, type);
                    _twitchBot.CommandCount++;
                    return;
                }
            }
        }
    }

    private void HandleAfkCommand(TwitchChatMessage chatMessage)
    {
        foreach (AfkCommandType type in AfkCommandTypes)
        {
            foreach (string alias in AppSettings.CommandList[type].Alias)
            {
                string? prefix = DbControl.Channels[chatMessage.ChannelId]?.Prefix;
                Regex pattern = PatternCreator.Create(alias, prefix);

                if (pattern.IsMatch(chatMessage.Message))
                {
                    if (CooldownController.IsOnAfkCooldown(chatMessage.UserId))
                    {
                        return;
                    }

                    AfkCommandHandler.Handle(chatMessage, type);
                    CooldownController.AddAfkCooldown(chatMessage.UserId);
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
        string? commandClassName = AppSettings.CommandList.GetCommandClassName(type);

        Type? commandClass = Type.GetType(commandClassName);
        if (commandClass is null)
        {
            throw new InvalidOperationException($"Could not get type of command class {commandClassName}");
        }

        ConstructorInfo? constructor = commandClass.GetConstructor(new[]
        {
            typeof(TwitchBot), typeof(TwitchChatMessage), typeof(string)
        });
        if (constructor is null)
        {
            throw new InvalidOperationException($"Could not instantiate command class {commandClassName}");
        }

        object handlerInstance = constructor.Invoke(new object[]
        {
            twitchBot, chatMessage, alias
        });

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
