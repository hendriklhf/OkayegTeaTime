using System.Text.RegularExpressions;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;
using JCommand = OkayegTeaTime.Files.JsonClasses.CommandData.Command;

namespace OkayegTeaTime.Twitch.Commands.CommandClasses;

public class CodeCommand : Command
{
    public CodeCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex pattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            JCommand? command = AppSettings.CommandList.FindCommand(ChatMessage.LowerSplit[1]);
            if (command is null)
            {
                Response = $"{ChatMessage.Username}, no matching command found";
                return;
            }

            Response = $"{ChatMessage.Username}, https://github.com/Sterbehilfe/OkayegTeaTimeCSharp/blob/master/OkayegTeaTime/Twitch/Commands/CommandClasses/{command.Name}Command.cs";
        }
    }
}
