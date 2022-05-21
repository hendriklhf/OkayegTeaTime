using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;
using JCommand = OkayegTeaTime.Files.Jsons.CommandData.Command;

namespace OkayegTeaTime.Twitch.Commands;

public class CodeCommand : Command
{
    public CodeCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex pattern = PatternCreator.Create(Alias, Prefix, @"\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            JCommand? command = _twitchBot.CommandController.FindCommand(ChatMessage.LowerSplit[1]);
            if (command is null)
            {
                Response = $"{ChatMessage.Username}, no matching command found";
                return;
            }

            Response = $"{ChatMessage.Username}, https://github.com/Sterbehilfe/OkayegTeaTime/blob/master/OkayegTeaTime/Twitch/Commands/{command.Name}Command.cs";
        }
    }
}
