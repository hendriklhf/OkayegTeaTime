using System;
using System.Collections.Frozen;
using System.Linq;
using OkayegTeaTime.Database.Cache.Enums;
using OkayegTeaTime.Models.Json;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Messages;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Controller;

public sealed class CommandController
{
    public Command[] Commands { get; }

    public AfkCommand[] AfkCommands { get; }

    private readonly FrozenSet<AliasHash> _afkCommandAliasHashes;

    public CommandController()
    {
        int commandTypeCount = Enum.GetValues<CommandType>().Length;
        Commands = new Command[commandTypeCount];
        foreach (Command command in AppSettings.CommandList.Commands)
        {
            CommandType commandType = Enum.Parse<CommandType>(command.Name, true);
            Commands[(int)commandType] = command;
        }

        int afkCommandTypeCount = Enum.GetValues<AfkType>().Length;
        AfkCommands = new AfkCommand[afkCommandTypeCount];
        foreach (AfkCommand command in AppSettings.CommandList.AfkCommands)
        {
            AfkType afkType = Enum.Parse<AfkType>(command.Name, true);
            AfkCommands[(int)afkType] = command;
        }

        _afkCommandAliasHashes = AfkCommands.SelectMany(c => c.Aliases).Select(a => new AliasHash(a)).ToFrozenSet(true);
    }

    public bool IsAfkCommand(string? channelPrefix, string message)
    {
        return MessageHelper.TryExtractAlias(message.AsMemory(), channelPrefix, out var alias, out _) && _afkCommandAliasHashes.Contains(new(alias.Span));
    }

    public Command GetCommand(CommandType type)
    {
        return Commands[(int)type];
    }

    public AfkCommand GetAfkCommand(AfkType type)
    {
        return AfkCommands[(int)type];
    }
}
