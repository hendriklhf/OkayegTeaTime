using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HLE.Twitch.Models;
using OkayegTeaTime.Database.Cache.Enums;
using OkayegTeaTime.Configuration;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Controller;
using OkayegTeaTime.Twitch.Messages;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Handlers;

public sealed class CommandHandler(TwitchBot twitchBot) : Handler(twitchBot)
{
    private readonly CommandExecutor _commandExecutor = new(twitchBot);
    private readonly AfkCommandHandler _afkCommandHandler = new(twitchBot);

    private static readonly FrozenDictionary<AliasHash, CommandType> s_commandTypes = CreateCommandTypeDictionary();
    private static readonly FrozenDictionary<AliasHash, AfkType> s_afkTypes = CreateAfkTypeDictionary();

    public override async ValueTask HandleAsync(IChatMessage chatMessage)
    {
        bool handled = await HandleCommandAsync(chatMessage);
        if (!handled)
        {
            await HandleAfkCommandAsync(chatMessage);
        }
    }

    private async ValueTask<bool> HandleCommandAsync(IChatMessage chatMessage)
    {
        ReadOnlyMemory<char> prefix = _twitchBot.Channels[chatMessage.ChannelId]?.Prefix?.AsMemory() ?? ReadOnlyMemory<char>.Empty;
        ReadOnlyMemory<char> prefixOrSuffix = prefix.Length == 0 ? GlobalSettings.Suffix.AsMemory() : prefix;

        if (!MessageHelpers.TryExtractAlias(chatMessage.Message.AsMemory(), prefix.Span, out ReadOnlyMemory<char> usedAlias, out ReadOnlyMemory<char> usedPrefixOrSuffix))
        {
            return false;
        }

        if (!prefixOrSuffix.Span.Equals(usedPrefixOrSuffix.Span, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        AliasHash aliasHash = new(usedAlias);
        if (!s_commandTypes.TryGetValue(aliasHash, out CommandType commandType))
        {
            return false;
        }

        if (_twitchBot.CooldownController.IsOnCooldown(chatMessage.UserId, commandType))
        {
            return false;
        }

        _twitchBot.CooldownController.AddCooldown(chatMessage.UserId, commandType);
        _twitchBot.CommandCount++;
        await _commandExecutor.ExecuteAsync(commandType, chatMessage, prefix, usedAlias);
        return true;
    }

    // ReSharper disable once InconsistentNaming
    private ValueTask HandleAfkCommandAsync(IChatMessage chatMessage)
    {
        ReadOnlyMemory<char> prefix = _twitchBot.Channels[chatMessage.ChannelId]?.Prefix?.AsMemory() ?? ReadOnlyMemory<char>.Empty;
        ReadOnlyMemory<char> prefixOrSuffix = prefix.Length == 0 ? GlobalSettings.Suffix.AsMemory() : prefix;

        if (!MessageHelpers.TryExtractAlias(chatMessage.Message.AsMemory(), prefix.Span, out ReadOnlyMemory<char> usedAlias, out ReadOnlyMemory<char> usedPrefixOrSuffix))
        {
            return ValueTask.CompletedTask;
        }

        if (!prefixOrSuffix.Span.Equals(usedPrefixOrSuffix.Span, StringComparison.OrdinalIgnoreCase))
        {
            return ValueTask.CompletedTask;
        }

        AliasHash aliasHash = new(usedAlias);
        if (!s_afkTypes.TryGetValue(aliasHash, out AfkType afkType))
        {
            return ValueTask.CompletedTask;
        }

        if (_twitchBot.CooldownController.IsOnAfkCooldown(chatMessage.UserId))
        {
            return ValueTask.CompletedTask;
        }

        _twitchBot.CooldownController.AddAfkCooldown(chatMessage.UserId);
        _twitchBot.CommandCount++;
        return _afkCommandHandler.HandleAsync(chatMessage, afkType);
    }

    private static FrozenDictionary<AliasHash, CommandType> CreateCommandTypeDictionary()
    {
        Dictionary<AliasHash, CommandType> result = [];
        CommandType[] handledCommands = typeof(CommandHandler).Assembly.GetTypes()
            .Where(static c => c.GetCustomAttribute<HandledCommandAttribute>() is not null)
            .Select(static c => c.GetCustomAttribute<HandledCommandAttribute>()!.CommandType).ToArray();
        foreach (Command command in CommandController.Commands)
        {
            CommandType type = Enum.Parse<CommandType>(command.Name);
            if (!handledCommands.Contains(type))
            {
                continue;
            }

            foreach (string alias in command.Aliases)
            {
                AliasHash aliasHashCode = new(alias.AsMemory());
                result.Add(aliasHashCode, type);
            }
        }

        return result.ToFrozenDictionary();
    }

    private static FrozenDictionary<AliasHash, AfkType> CreateAfkTypeDictionary()
    {
        Dictionary<AliasHash, AfkType> result = [];
        foreach (AfkCommand afkCommand in CommandController.AfkCommands)
        {
            AfkType type = Enum.Parse<AfkType>(afkCommand.Name, true);
            foreach (string alias in afkCommand.Aliases)
            {
                AliasHash aliasHashCode = new(alias.AsMemory());
                result.Add(aliasHashCode, type);
            }
        }

        return result.ToFrozenDictionary();
    }
}
