using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HLE.Collections;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Cache;
using OkayegTeaTime.Database.Cache.Enums;
using OkayegTeaTime.Files;
using OkayegTeaTime.Files.Models;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Controller;

public class CommandController
{
    public IEnumerable<string> CommandAliases => GetCommandAliases();

    public IEnumerable<string> AfkCommandAliases => GetAfkCommandAliases();

    public Command this[CommandType type] => GetCommand(type);

    public AfkCommand this[AfkType type] => GetAfkCommand(type);

    public IEnumerable<Command> Commands { get; }

    public IEnumerable<AfkCommand> AfkCommands { get; }

    private readonly ChannelCache? _channelCache;
    private IEnumerable<string>? _commandAliases;
    private IEnumerable<string>? _afkCommandAliases;

    public CommandController(ChannelCache? channelCache = null)
    {
        _channelCache = channelCache;
        Commands = JsonController.GetCommandList().Commands.OrderBy(c => c.Name).ForEach(c => c.Aliases = c.Aliases.OrderBy(a => a).ToArray());
        AfkCommands = JsonController.GetCommandList().AfkCommands.OrderBy(c => c.Name).ForEach(c => c.Aliases = c.Aliases.OrderBy(a => a).ToArray());
    }

    public bool IsAfkCommand(TwitchChatMessage chatMessage)
    {
        string? prefix = _channelCache is null ? DbController.GetChannel(chatMessage.ChannelId)?.Prefix : _channelCache[chatMessage.ChannelId]?.Prefix;
        return AfkCommandAliases.Any(alias =>
        {
            Regex pattern = PatternCreator.Create(alias, prefix);
            return pattern.IsMatch(chatMessage.Message);
        });
    }

    public Command? FindCommand(string searchValue)
    {
        Regex pattern = new($"^{searchValue}$", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));
        return Commands.FirstOrDefault(c => pattern.IsMatch(c.Name) || c.Aliases.Any(a => pattern.IsMatch(a)));
    }

    private Command GetCommand(CommandType type)
    {
        Command? command = Commands.FirstOrDefault(c => string.Equals(c.Name, type.ToString(), StringComparison.OrdinalIgnoreCase));
        return command ?? throw new ArgumentNullException(nameof(command));
    }

    private AfkCommand GetAfkCommand(AfkType type)
    {
        AfkCommand? command = AfkCommands.FirstOrDefault(c => string.Equals(c.Name, type.ToString(), StringComparison.OrdinalIgnoreCase));
        return command ?? throw new ArgumentNullException(nameof(command));
    }

    private IEnumerable<string> GetCommandAliases()
    {
        if (_commandAliases is not null)
        {
            return _commandAliases;
        }

        _commandAliases = Commands.Select(c => c.Aliases).SelectEach();
        return _commandAliases;
    }

    private IEnumerable<string> GetAfkCommandAliases()
    {
        if (_afkCommandAliases is not null)
        {
            return _afkCommandAliases;
        }

        _afkCommandAliases = AfkCommands.Select(c => c.Aliases).SelectEach();
        return _afkCommandAliases;
    }
}
