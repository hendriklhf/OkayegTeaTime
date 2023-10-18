using System;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using OkayegTeaTime.Database.Cache.Enums;
using OkayegTeaTime.Models.Json;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Messages;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Controller;

public sealed class CommandController
{
    public ImmutableArray<Command> Commands { get; }

    public ImmutableArray<AfkCommand> AfkCommands { get; }

    private readonly FrozenSet<AliasHash> _afkCommandAliasHashes;

    public CommandController()
    {
        int commandTypeCount = Enum.GetValues<CommandType>().Length;
        Command[] commands = new Command[commandTypeCount];
        Commands = ImmutableCollectionsMarshal.AsImmutableArray(commands);
        foreach (Command command in AppSettings.CommandList.Commands)
        {
            CommandType commandType = Enum.Parse<CommandType>(command.Name, true);
            commands[(int)commandType] = command;
        }

        int afkCommandTypeCount = Enum.GetValues<AfkType>().Length;
        AfkCommand[] afkCommands = new AfkCommand[afkCommandTypeCount];
        AfkCommands = ImmutableCollectionsMarshal.AsImmutableArray(afkCommands);
        foreach (AfkCommand command in AppSettings.CommandList.AfkCommands)
        {
            AfkType afkType = Enum.Parse<AfkType>(command.Name, true);
            afkCommands[(int)afkType] = command;
        }

        _afkCommandAliasHashes = AfkCommands.SelectMany(static c => c.Aliases).Select(static a => new AliasHash(a)).ToFrozenSet();
    }

    public bool IsAfkCommand(string? channelPrefix, string message)
        => MessageHelper.TryExtractAlias(message.AsMemory(), channelPrefix, out var alias, out _) && _afkCommandAliasHashes.Contains(new(alias.Span));

    public Command GetCommand(CommandType type) => Commands[(int)type];

    public AfkCommand GetAfkCommand(AfkType type) => AfkCommands[(int)type];
}
