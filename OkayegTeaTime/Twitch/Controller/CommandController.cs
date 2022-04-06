using HLE.Collections;
using OkayegTeaTime.Database;
using OkayegTeaTime.Files;
using OkayegTeaTime.Files.Jsons.CommandData;
using OkayegTeaTime.Twitch.Commands.Enums;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Controller;

public class CommandController
{
    public IEnumerable<string> CommandAliases => GetCommandAliases();

    public IEnumerable<string> AfkCommandAliases => GetAfkCommandAliases();

    public Command this[CommandType type] => GetCommand(type);

    public AfkCommand this[AfkCommandType type] => GetAfkCommand(type);

    public IEnumerable<Command> Commands { get; }

    public IEnumerable<AfkCommand> AfkCommands { get; }

    private IEnumerable<string>? _commandAliases;
    private IEnumerable<string>? _afkCommandAliases;

    public CommandController()
    {
        Commands = JsonController.GetCommandList().Commands;
        AfkCommands = JsonController.GetCommandList().AfkCommands;
    }

    public bool IsAfkCommand(TwitchChatMessage chatMessage)
    {
        string? prefix = DbControl.Channels[chatMessage.ChannelId]?.Prefix;
        return AfkCommandAliases.Any(alias =>
        {
            Regex pattern = PatternCreator.Create(alias, prefix);
            return pattern.IsMatch(chatMessage.Message);
        });
    }

    public Command? FindCommand(string searchValue)
    {
        Regex pattern = new($"^{searchValue}$", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));
        return Commands.FirstOrDefault(c => pattern.IsMatch(c.Name) || c.Alias.Any(a => pattern.IsMatch(a)));
    }

    private Command GetCommand(CommandType type)
    {
        Command? command = Commands.FirstOrDefault(c => string.Equals(c.Name, type.ToString(), StringComparison.OrdinalIgnoreCase));
        return command ?? throw new ArgumentNullException(nameof(command));
    }

    private AfkCommand GetAfkCommand(AfkCommandType type)
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

        _commandAliases = Commands.Select(c => c.Alias).SelectEach();
        return _commandAliases;
    }

    private IEnumerable<string> GetAfkCommandAliases()
    {
        if (_afkCommandAliases is not null)
        {
            return _afkCommandAliases;
        }

        _afkCommandAliases = AfkCommands.Select(c => c.Alias).SelectEach();
        return _afkCommandAliases;
    }
}
