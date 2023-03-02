using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using HLE.Twitch.Models;
using OkayegTeaTime.Database.Cache.Enums;
using OkayegTeaTime.Models.Json;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Controller;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Handlers;

public sealed class CommandHandler : Handler
{
    private readonly CommandExecutor _commandExecutor;
    private readonly AfkCommandHandler _afkCommandHandler;
    private readonly CooldownController _cooldownController;

    private readonly FrozenDictionary<int, CommandType> _commandTypes;
    private readonly AfkType[] _afkTypes = Enum.GetValues<AfkType>();

    public CommandHandler(TwitchBot twitchBot) : base(twitchBot)
    {
        _afkCommandHandler = new(twitchBot);
        _cooldownController = new(twitchBot.CommandController);
        _commandTypes = CreateCommandTypeDictionary(twitchBot);
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
        ExtractAlias(chatMessage.Message.AsMemory(), prefix, out var usedAlias, out var usedPrefix);

        if (!prefixOrSuffix.Equals(usedPrefix.Span, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        int aliasHashCode = string.GetHashCode(usedAlias.Span, StringComparison.OrdinalIgnoreCase);
        if (!_commandTypes.TryGetValue(aliasHashCode, out CommandType type))
        {
            return false;
        }

        if (_cooldownController.IsOnCooldown(chatMessage.UserId, type))
        {
            return false;
        }

        _twitchBot.CommandCount++;
        _commandExecutor.Execute(type, chatMessage, prefix, usedAlias.Span);
        _cooldownController.AddCooldown(chatMessage.UserId, type);
        return true;
    }

    private void HandleAfkCommand(ChatMessage chatMessage)
    {
        // TODO: change to handling like normal command
        string? prefix = _twitchBot.Channels[chatMessage.ChannelId]?.Prefix;
        Span<AfkType> afkTypes = _afkTypes;
        int afkTypesLength = _afkTypes.Length;
        for (int i = 0; i < afkTypesLength; i++)
        {
            AfkType type = afkTypes[i];
            Span<string> aliases = _twitchBot.CommandController[type].Aliases;
            int aliasesLength = aliases.Length;
            for (int j = 0; j < aliasesLength; j++)
            {
                string alias = aliases[j];
                Regex pattern = _twitchBot.RegexCreator.Create(alias, prefix);
                if (!pattern.IsMatch(chatMessage.Message))
                {
                    continue;
                }

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

    private static FrozenDictionary<int, CommandType> CreateCommandTypeDictionary(TwitchBot twitchBot)
    {
        Dictionary<int, CommandType> result = new();
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
                int aliasHashCode = string.GetHashCode(alias, StringComparison.OrdinalIgnoreCase);
                result.Add(aliasHashCode, type);
            }
        }

        return result.ToFrozenDictionary();
    }

    private static void ExtractAlias(ReadOnlyMemory<char> message, ReadOnlySpan<char> prefix, out ReadOnlyMemory<char> usedAlias, out ReadOnlyMemory<char> usedPrefix)
    {
        ReadOnlySpan<char> messageSpan = message.Span;
        int indexOfWhitespace = messageSpan.IndexOf(' ');
        ReadOnlyMemory<char> firstWord = message[..Unsafe.As<int, Index>(ref indexOfWhitespace)];
        if (firstWord.Length <= (prefix.Length == 0 ? AppSettings.Suffix.Length : prefix.Length))
        {
            usedAlias = ReadOnlyMemory<char>.Empty;
            usedPrefix = ReadOnlyMemory<char>.Empty;
            return;
        }

        if (prefix.Length == 0)
        {
            usedAlias = firstWord[..^AppSettings.Suffix.Length];
            usedPrefix = firstWord[^AppSettings.Suffix.Length..];
            return;
        }

        usedAlias = firstWord[prefix.Length..];
        usedPrefix = firstWord[..prefix.Length];
    }
}
