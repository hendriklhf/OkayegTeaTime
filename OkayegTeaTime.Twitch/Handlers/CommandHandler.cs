using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using OkayegTeaTime.Database.Cache.Enums;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Commands;
using OkayegTeaTime.Twitch.Controller;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Handlers;

public sealed class CommandHandler : Handler
{
    private readonly AfkCommandHandler _afkCommandHandler;
    private readonly CooldownController _cooldownController;

    private readonly CommandType[] _commandTypes = Enum.GetValues<CommandType>();
    private readonly AfkType[] _afkTypes = Enum.GetValues<AfkType>();
    private readonly Dictionary<CommandType, CommandHandle> _commandHandles;

    public CommandHandler(TwitchBot twitchBot) : base(twitchBot)
    {
        _afkCommandHandler = new(twitchBot);
        _cooldownController = new(_twitchBot.CommandController);
        _commandHandles = BuildCommandCache();
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
        string? prefix = _twitchBot.Channels[chatMessage.ChannelId]?.Prefix;
        foreach (CommandType type in _commandTypes)
        {
            foreach (string alias in _twitchBot.CommandController[type].Aliases)
            {
                Regex pattern = PatternCreator.Create(alias, prefix);
                if (pattern.IsMatch(chatMessage.Message))
                {
                    if (_cooldownController.IsOnCooldown(chatMessage.UserId, type))
                    {
                        return false;
                    }

                    _twitchBot.CommandCount++;
                    InvokeCommandHandle(type, chatMessage, alias);
                    _cooldownController.AddCooldown(chatMessage.UserId, type);
                    return true;
                }
            }
        }

        return false;
    }

    private void HandleAfkCommand(TwitchChatMessage chatMessage)
    {
        string? prefix = _twitchBot.Channels[chatMessage.ChannelId]?.Prefix;
        foreach (AfkType type in _afkTypes)
        {
            foreach (string alias in _twitchBot.CommandController[type].Aliases)
            {
                Regex pattern = PatternCreator.Create(alias, prefix);
                if (pattern.IsMatch(chatMessage.Message))
                {
                    if (_cooldownController.IsOnAfkCooldown(chatMessage.UserId))
                    {
                        return;
                    }

                    _twitchBot.CommandCount++;
                    _afkCommandHandler.Handle(chatMessage, type);
                    _cooldownController.AddAfkCooldown(chatMessage.UserId);
                    return;
                }
            }
        }
    }

    /// <summary>
    ///     Attempts to handle a command through a handler via reflection
    /// </summary>
    /// <param name="type">The command handler class type</param>
    /// <param name="chatMessage">The chat message to handle</param>
    /// <param name="alias">A command alias</param>
    /// <exception cref="InvalidOperationException">The command handler doesn't conform</exception>
    private void InvokeCommandHandle(CommandType type, TwitchChatMessage chatMessage, string alias)
    {
        CommandHandle handle = _commandHandles[type];
        Command command = handle.CreateCommandInstance(_twitchBot, chatMessage, alias);
        command.Handle();
        string response = command.Response;
        if (response.IsNullOrEmptyOrWhitespace())
        {
            return;
        }

        _twitchBot.Send(chatMessage.Channel, response);
    }

    private static Dictionary<CommandType, CommandHandle> BuildCommandCache()
    {
        Dictionary<CommandType, CommandHandle> commandHandles = new();
        Type[] commands = Assembly.GetCallingAssembly().GetTypes().Where(t => t.GetCustomAttribute<HandledCommand>() is not null).ToArray();
        foreach (Type command in commands)
        {
            ConstructorInfo? constructor = command.GetConstructor(new[]
            {
                typeof(TwitchBot),
                typeof(TwitchChatMessage),
                typeof(string)
            });
            if (constructor is null)
            {
                throw new InvalidOperationException($"Could not get constructor for class {command.FullName}");
            }

            CommandHandle handle = new(constructor);
            commandHandles.Add(command.GetCustomAttribute<HandledCommand>()!.CommandType, handle);
        }

        return commandHandles;
    }
}
