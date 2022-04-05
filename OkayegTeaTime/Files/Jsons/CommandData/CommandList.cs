using System.Text.Json.Serialization;
using HLE.Strings;
using OkayegTeaTime.Twitch.Commands.Enums;

#nullable disable

namespace OkayegTeaTime.Files.Jsons.CommandData;

public class CommandList
{
    public List<Command> Commands { get; set; }

    public List<AfkCommand> AfkCommands { get; set; }

    [JsonIgnore]
    public IEnumerable<string> AfkCommandAliases
    {
        get
        {
            if (_afkCommandAliases is not null)
            {
                return _afkCommandAliases;
            }

            List<string> listAlias = new();
            AppSettings.CommandList.AfkCommands.ForEach(cmd => cmd.Alias.ForEach(alias => listAlias.Add(alias)));
            _afkCommandAliases = listAlias;
            return listAlias;
        }
    }

    [JsonIgnore]
    public IEnumerable<string> CommandAliases
    {
        get
        {
            if (_commandAliases is not null)
            {
                return _commandAliases;
            }

            List<string> listAlias = new();
            AppSettings.CommandList.Commands.ForEach(cmd => cmd.Alias.ForEach(alias => listAlias.Add(alias)));
            _commandAliases = listAlias;
            return listAlias;
        }
    }

    [JsonIgnore]
    public IEnumerable<string> AllAliases
    {
        get
        {
            if (_allAliases is not null)
            {
                return _allAliases;
            }

            List<string> listAlias = CommandAliases.Concat(AfkCommandAliases).ToList();
            _allAliases = listAlias;
            return listAlias;
        }
    }

    [JsonIgnore]
    public AfkCommand this[AfkCommandType type] => AfkCommands.FirstOrDefault(cmd => string.Equals(cmd.Name, type.ToString(), StringComparison.CurrentCultureIgnoreCase));

    [JsonIgnore]
    public Command this[CommandType type] => Commands.FirstOrDefault(cmd => string.Equals(cmd.Name, type.ToString(), StringComparison.CurrentCultureIgnoreCase));

    [JsonIgnore]
    private IEnumerable<string> _afkCommandAliases;
    [JsonIgnore]
    private IEnumerable<string> _commandAliases;
    [JsonIgnore]
    private IEnumerable<string> _allAliases;

    public string GetCommandClassName(CommandType type)
    {
        return $"{AppSettings.AssemblyName}.Twitch.Commands.{type}Command";
    }

#nullable enable
    public Command? FindCommand(string searchValue)
    {
        return Commands.FirstOrDefault(c => c.Name.IsMatch($"^{searchValue}$") || c.Alias.Any(a => a.IsMatch($"^{searchValue}$")));
    }
}
