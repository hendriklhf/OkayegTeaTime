using System;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using OkayegTeaTime.Database.Cache.Enums;
using OkayegTeaTime.Twitch.Messages;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Controller;

public static class CommandController
{
    public static ImmutableArray<Command> Commands => CommandDataStorage.Commands;

    public static ImmutableArray<AfkCommand> AfkCommands => CommandDataStorage.AfkCommands;

    private static readonly FrozenSet<AliasHash> s_afkCommandAliasHashes = CommandDataStorage.AfkCommands
        .SelectMany(static c => c.Aliases)
        .Select(static a => new AliasHash(a.AsMemory()))
        .ToFrozenSet();

    [Pure]
    public static bool IsAfkCommand(string? channelPrefix, ReadOnlyMemory<char> message)
        => MessageHelpers.TryExtractAlias(message, channelPrefix, out ReadOnlyMemory<char> usedAlias, out _) &&
           s_afkCommandAliasHashes.Contains(new(usedAlias));

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Command GetCommand(CommandType type)
    {
        Command[] commands = ImmutableCollectionsMarshal.AsArray(CommandDataStorage.Commands)!;
        return Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(commands), (int)type);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AfkCommand GetAfkCommand(AfkType type)
    {
        AfkCommand[] afkCommands = ImmutableCollectionsMarshal.AsArray(CommandDataStorage.AfkCommands)!;
        return Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(afkCommands), (int)type);
    }
}
