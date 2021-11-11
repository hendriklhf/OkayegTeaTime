using System.Text.Json.Serialization;
using OkayegTeaTimeCSharp.Twitch.Commands.Enums;
using OkayegTeaTimeCSharp.Twitch.Messages.Interfaces;

namespace OkayegTeaTimeCSharp.JsonData.JsonClasses.CommandData;

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
            JsonController.CommandList.AfkCommands.ForEach(cmd => cmd.Alias.ForEach(alias => listAlias.Add(alias)));
            return listAlias;
        }
    }

    [JsonIgnore]
    public List<string> CommandAliases
    {
        get
        {
            List<string> listAlias = new();
            JsonController.CommandList.Commands.ForEach(cmd => cmd.Alias.ForEach(alias => listAlias.Add(alias)));
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
        return this[type].Alias.Any(alias => chatMessage.Channel.Prefix + alias == chatMessage.LowerSplit[0] || alias + Config.Suffix == chatMessage.LowerSplit[0]);
    }

    public bool MatchesAnyAlias(ITwitchChatMessage chatMessage, AfkCommandType type)
    {
        return this[type].Alias.Any(alias => chatMessage.Channel.Prefix + alias == chatMessage.LowerSplit[0] || alias + Config.Suffix == chatMessage.LowerSplit[0]);
    }

    public string GetCommandClassName(CommandType type)
    {
        return $"OkayegTeaTimeCSharp.Twitch.Commands.CommandClasses.{type}Command";
    }
}
