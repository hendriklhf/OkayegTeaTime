using System.Text.Json.Serialization;
using HLE.Strings;
using OkayegTeaTime.Twitch.Commands.Enums;

#nullable disable

namespace OkayegTeaTime.Files.JsonClasses.CommandData;

public class CommandList
{
    public List<Command> Commands { get; set; }

    public List<AfkCommand> AfkCommands { get; set; }

    [JsonIgnore]
    public List<string> AfkCommandAliases
    {
        get
        {
            if (_afkCommandAliases is not null)
            {
                return _afkCommandAliases;
            }

            List<string> listAlias = new();
            AppSettings.CommandList.AfkCommands.ForEach(cmd => cmd?.Alias?.ForEach(alias => listAlias.Add(alias)));
            _afkCommandAliases = listAlias;
            return listAlias;
        }
    }

    [JsonIgnore]
    public List<string> CommandAliases
    {
        get
        {
            if (_commandAliases is not null)
            {
                return _commandAliases;
            }

            List<string> listAlias = new();
            AppSettings.CommandList.Commands.ForEach(cmd => cmd?.Alias?.ForEach(alias => listAlias.Add(alias)));
            _commandAliases = listAlias;
            return listAlias;
        }
    }

    [JsonIgnore]
    public List<string> AllAliases
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
    public AfkCommand this[AfkCommandType type] => AfkCommands.FirstOrDefault(cmd => cmd.Name.ToLower() == type.ToString().ToLower());

    [JsonIgnore]
    public Command this[CommandType type] => Commands.FirstOrDefault(cmd => cmd.Name.ToLower() == type.ToString().ToLower());

    [JsonIgnore]
    private List<string> _afkCommandAliases;
    [JsonIgnore]
    private List<string> _commandAliases;
    [JsonIgnore]
    private List<string> _allAliases;

    public string GetCommandClassName(CommandType type)
    {
        return $"{AppSettings.AssemblyName}.Twitch.Commands.CommandClasses.{type}Command";
    }

#nullable enable
    public Command? FindCommand(string searchValue)
    {
        return Commands.FirstOrDefault(c => c.Name.IsMatch($"^{searchValue}$") || c.Alias.Any(a => a.IsMatch($"^{searchValue}$")));
    }
}
