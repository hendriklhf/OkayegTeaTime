using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using HLE.Collections;
using OkayegTeaTime.Database.Cache.Enums;
using OkayegTeaTime.Files;
using OkayegTeaTime.Files.Models;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Controller;

public sealed class CommandController
{
    public Command this[CommandType type] => GetCommand(type);

    public AfkCommand this[AfkType type] => GetAfkCommand(type);

    public Command[] Commands { get; }

    public AfkCommand[] AfkCommands { get; }

    private readonly string[] _afkCommandAliases;

    public CommandController()
    {
        Commands = JsonController.GetCommandList().Commands.OrderBy(c => c.Name).ForEach(c => c.Aliases = c.Aliases.Order().ToArray()).ToArray();
        AfkCommands = JsonController.GetCommandList().AfkCommands.OrderBy(c => c.Name).ForEach(c => c.Aliases = c.Aliases.Order().ToArray()).ToArray();
        _afkCommandAliases = AfkCommands.SelectMany(c => c.Aliases).ToArray();
    }

    public bool IsAfkCommand(string? prefix, string message)
    {
        ref string firstAlias = ref MemoryMarshal.GetArrayDataReference(_afkCommandAliases);
        for (int i = 0; i < firstAlias.Length; i++)
        {
            string alias = Unsafe.Add(ref firstAlias, i);
            Regex pattern = PatternCreator.Create(alias, prefix);
            if (pattern.IsMatch(message))
            {
                return true;
            }
        }

        return false;
    }

    private Command GetCommand(CommandType type)
    {
        string typeName = type.ToString();
        ref Command firstCommand = ref MemoryMarshal.GetArrayDataReference(Commands);
        for (int i = 0; i < Commands.Length; i++)
        {
            Command command = Unsafe.Add(ref firstCommand, i);
            if (command.Name == typeName)
            {
                return command;
            }
        }

        throw new InvalidOperationException("Command not found. This should not happen.");
    }

    private AfkCommand GetAfkCommand(AfkType type)
    {
        string typeName = type.ToString().ToLower();
        Span<AfkCommand> afkCommands = AfkCommands;
        ref AfkCommand firstAfkCommands = ref afkCommands[0];
        for (int i = 0; i < afkCommands.Length; i++)
        {
            AfkCommand afkCommand = Unsafe.Add(ref firstAfkCommands, i);
            if (afkCommand.Name == typeName)
            {
                return afkCommand;
            }
        }

        throw new InvalidOperationException("AfkCommand not found. This should not happen.");
    }
}
