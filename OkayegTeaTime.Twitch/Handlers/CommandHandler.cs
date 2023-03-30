using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HLE.Twitch.Models;
using OkayegTeaTime.Database.Cache.Enums;
using OkayegTeaTime.Models.Json;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Messages;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Handlers;

public sealed class CommandHandler : Handler
{
    private readonly CommandExecutor _commandExecutor;
    private readonly AfkCommandHandler _afkCommandHandler;

    private readonly FrozenDictionary<AliasHash, CommandType> _commandTypes;
    private readonly FrozenDictionary<AliasHash, AfkType> _afkTypes;

    public CommandHandler(TwitchBot twitchBot) : base(twitchBot)
    {
        _afkCommandHandler = new(twitchBot);
        _commandTypes = CreateCommandTypeDictionary(twitchBot);
        _afkTypes = CreateAfkTypeDictionary(twitchBot);
        _commandExecutor = new(twitchBot);
    }

    public override void Handle(ChatMessage chatMessage)
    {
        bool handled = HandleCommand(chatMessage);
        if (!handled)
        {
            HandleAfkCommand(chatMessage);
        }
    }

    private bool HandleCommand(ChatMessage chatMessage)
    {
        ReadOnlySpan<char> prefix = _twitchBot.Channels[chatMessage.ChannelId]?.Prefix;
        ReadOnlySpan<char> prefixOrSuffix = prefix.Length == 0 ? AppSettings.Suffix : prefix;
        MessageHelper.ExtractAlias(chatMessage.Message.AsMemory(), prefix, out var usedAlias, out var usedPrefix);

        if (!prefixOrSuffix.Equals(usedPrefix.Span, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        AliasHash aliasHashCode = new(usedAlias.Span);
        if (!_commandTypes.TryGetValue(aliasHashCode, out CommandType type))
        {
            return false;
        }

        if (_twitchBot.CooldownController.IsOnCooldown(chatMessage.UserId, type))
        {
            return false;
        }

        _twitchBot.CommandCount++;
        _commandExecutor.Execute(type, chatMessage, prefix, usedAlias.Span);
        _twitchBot.CooldownController.AddCooldown(chatMessage.UserId, type);
        return true;
    }

    private void HandleAfkCommand(ChatMessage chatMessage)
    {
        ReadOnlySpan<char> prefix = _twitchBot.Channels[chatMessage.ChannelId]?.Prefix;
        ReadOnlySpan<char> prefixOrSuffix = prefix.Length == 0 ? AppSettings.Suffix : prefix;
        MessageHelper.ExtractAlias(chatMessage.Message.AsMemory(), prefix, out var usedAlias, out var usedPrefix);

        if (!prefixOrSuffix.Equals(usedPrefix.Span, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        AliasHash aliasHashCode = new(usedAlias.Span);
        if (!_afkTypes.TryGetValue(aliasHashCode, out AfkType type))
        {
            return;
        }

        if (_twitchBot.CooldownController.IsOnAfkCooldown(chatMessage.UserId))
        {
            return;
        }

        _twitchBot.CommandCount++;
        _afkCommandHandler.Handle(chatMessage, type);
        _twitchBot.CooldownController.AddAfkCooldown(chatMessage.UserId);
    }

    private static FrozenDictionary<AliasHash, CommandType> CreateCommandTypeDictionary(TwitchBot twitchBot)
    {
        Dictionary<AliasHash, CommandType> result = new();
        CommandType[] handledCommands = Assembly.GetExecutingAssembly().GetTypes()
            .Where(c => c.GetCustomAttribute<HandledCommandAttribute>() is not null)
            .Select(c => c.GetCustomAttribute<HandledCommandAttribute>()!.CommandType).ToArray();
        foreach (Command command in twitchBot.CommandController.Commands)
        {
            CommandType type = Enum.Parse<CommandType>(command.Name);
            if (!handledCommands.Contains(type))
            {
                continue;
            }

            foreach (string alias in command.Aliases)
            {
                AliasHash aliasHashCode = new(alias);
                result.Add(aliasHashCode, type);
            }
        }

        return result.ToFrozenDictionary();
    }

    private static FrozenDictionary<AliasHash, AfkType> CreateAfkTypeDictionary(TwitchBot twitchBot)
    {
        Dictionary<AliasHash, AfkType> result = new();
        foreach (AfkCommand afkCommand in twitchBot.CommandController.AfkCommands)
        {
            AfkType type = Enum.Parse<AfkType>(afkCommand.Name, true);
            foreach (string alias in afkCommand.Aliases)
            {
                AliasHash aliasHashCode = new(alias);
                result.Add(aliasHashCode, type);
            }
        }

        return result.ToFrozenDictionary();
    }
}
