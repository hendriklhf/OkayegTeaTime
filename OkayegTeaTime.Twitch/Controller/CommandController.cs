using System;
using System.Linq;
using System.Text.RegularExpressions;
using HLE.Collections;
using OkayegTeaTime.Database.Cache.Enums;
using OkayegTeaTime.Files;
using OkayegTeaTime.Files.Models;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Controller;

public class CommandController
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
        return _afkCommandAliases.Any(alias =>
        {
            Regex pattern = PatternCreator.Create(alias, prefix);
            return pattern.IsMatch(message);
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
}