using System.Text.Json.Serialization;
using OkayegTeaTime.Twitch.Commands.Enums;
using OkayegTeaTime.Twitch.Messages.Interfaces;

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
            List<string> listAlias = new();
            AppSettings.CommandList.AfkCommands.ForEach(cmd => cmd.Alias.ForEach(alias => listAlias.Add(alias)));
            return listAlias;
        }
    }

    [JsonIgnore]
    public List<string> CommandAliases
    {
        get
        {
            List<string> listAlias = new();
            AppSettings.CommandList.Commands.ForEach(cmd => cmd.Alias.ForEach(alias => listAlias.Add(alias)));
            return listAlias;
        }
    }

    [JsonIgnore]
    public List<string> AllAliases => CommandAliases.Concat(AfkCommandAliases).ToList();

    [JsonIgnore]
    public AfkCommand this[AfkCommandType type] => AfkCommands.FirstOrDefault(cmd => cmd.CommandName == type.ToString().ToLower());

    [JsonIgnore]
    public Command this[CommandType type] => Commands.FirstOrDefault(cmd => cmd.CommandName == type.ToString().ToLower());

    public bool MatchesAnyAlias(ITwitchChatMessage chatMessage, CommandType type)
    {
        return this[type].Alias.Any(alias => chatMessage.Channel.Prefix + alias == chatMessage.LowerSplit[0] || alias + AppSettings.Suffix == chatMessage.LowerSplit[0]);
    }

    public bool MatchesAnyAlias(ITwitchChatMessage chatMessage, AfkCommandType type)
    {
        return this[type].Alias.Any(alias => chatMessage.Channel.Prefix + alias == chatMessage.LowerSplit[0] || alias + AppSettings.Suffix == chatMessage.LowerSplit[0]);
    }

    public string GetCommandClassName(CommandType type)
    {
        return $"{AppSettings.AssemblyName}.Twitch.Commands.CommandClasses.{type}Command";
    }
}
