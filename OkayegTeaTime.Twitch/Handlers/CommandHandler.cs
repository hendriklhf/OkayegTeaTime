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
    private readonly CommandExecutor _commandExecutor = new();
    private readonly AfkCommandHandler _afkCommandHandler;
    private readonly CooldownController _cooldownController;

    private readonly CommandType[] _commandTypes;
    private readonly AfkType[] _afkTypes = Enum.GetValues<AfkType>();

    public CommandHandler(TwitchBot twitchBot) : base(twitchBot)
    {
        _afkCommandHandler = new(twitchBot);
        _cooldownController = new(_twitchBot.CommandController);
        _commandTypes = GetHandledCommandTypes();
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
                    _commandExecutor.Execute(type, _twitchBot, chatMessage, prefix, alias);
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

    private static CommandType[] GetHandledCommandTypes()
    {
        List<CommandType> commandTypes = new();
        Type[] commands = Assembly.GetCallingAssembly().GetTypes().Where(t => t.GetCustomAttribute<HandledCommand>() is not null).ToArray();
        foreach (Type command in commands)
        {
            commandTypes.Add(command.GetCustomAttribute<HandledCommand>()!.CommandType);
        }

        return commandTypes.ToArray();
    }
}
