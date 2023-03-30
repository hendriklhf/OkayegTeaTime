using System;
using System.Collections.Frozen;
using System.Diagnostics;
using System.Linq;
using OkayegTeaTime.Database.Cache.Enums;
using OkayegTeaTime.Models.Json;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Messages;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Controller;

public sealed class CommandController
{
    public Command this[CommandType type] => GetCommand(type);

    public AfkCommand this[AfkType type] => GetAfkCommand(type);

    public Command[] Commands { get; }

    public AfkCommand[] AfkCommands { get; }

    private readonly FrozenSet<AliasHash> _afkCommandAliasHashes;

    public CommandController()
    {
        Commands = AppSettings.CommandList.Commands.OrderBy(c => c.Name).ToArray();

        AfkCommands = AppSettings.CommandList.AfkCommands;
        _afkCommandAliasHashes = AfkCommands.SelectMany(c => c.Aliases).Select(a => new AliasHash(a)).ToFrozenSet();
    }

    public bool IsAfkCommand(string? prefix, string message)
    {
        MessageHelper.ExtractAlias(message.AsMemory(), prefix, out var alias, out _);
        return _afkCommandAliasHashes.Contains(new(alias.Span));
    }

    private Command GetCommand(CommandType type)
    {
        // asserting the enum and the commands are both in the same order so commands can be accessed by index of the enum value
        Debug.Assert(Enum.GetNames<CommandType>().Select(c => c.ToLower()).SequenceEqual(Commands.Select(c => c.Name.ToLower())));

        return Commands[(int)type];
    }

    private AfkCommand GetAfkCommand(AfkType type)
    {
        // asserting the enum and the commands are both in the same order so commands can be accessed by index of the enum value
        Debug.Assert(Enum.GetNames<AfkType>().Select(a => a.ToLower()).SequenceEqual(AfkCommands.Select(a => a.Name.ToLower())));

        return AfkCommands[(int)type];
    }
}
