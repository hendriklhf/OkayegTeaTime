using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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

    public override async ValueTask Handle(ChatMessage chatMessage)
    {
        bool handled = await HandleCommand(chatMessage);
        if (!handled)
        {
            await HandleAfkCommand(chatMessage);
        }
    }

    private async ValueTask<bool> HandleCommand(ChatMessage chatMessage)
    {
        ReadOnlyMemory<char> prefix = _twitchBot.Channels[chatMessage.ChannelId]?.Prefix.AsMemory() ?? ReadOnlyMemory<char>.Empty;
        ReadOnlyMemory<char> prefixOrSuffix = prefix.Length == 0 ? AppSettings.Suffix.AsMemory() : prefix;
        MessageHelper.ExtractAlias(chatMessage.Message.AsMemory(), prefix.Span, out var usedAlias, out var usedPrefix);

        if (!prefixOrSuffix.Span.Equals(usedPrefix.Span, StringComparison.OrdinalIgnoreCase))
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
        await _commandExecutor.Execute(type, chatMessage, prefix, usedAlias);
        _twitchBot.CooldownController.AddCooldown(chatMessage.UserId, type);
        return true;
    }

    private async ValueTask HandleAfkCommand(ChatMessage chatMessage)
    {
        ReadOnlyMemory<char> prefix = _twitchBot.Channels[chatMessage.ChannelId]?.Prefix.AsMemory() ?? ReadOnlyMemory<char>.Empty;
        ReadOnlyMemory<char> prefixOrSuffix = prefix.Length == 0 ? AppSettings.Suffix.AsMemory() : prefix;
        MessageHelper.ExtractAlias(chatMessage.Message.AsMemory(), prefix.Span, out var usedAlias, out var usedPrefix);

        if (!prefixOrSuffix.Span.Equals(usedPrefix.Span, StringComparison.OrdinalIgnoreCase))
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
        await _afkCommandHandler.Handle(chatMessage, type);
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

        return result.ToFrozenDictionary(true);
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

        return result.ToFrozenDictionary(true);
    }
}
