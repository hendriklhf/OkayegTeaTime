using System.Text.RegularExpressions;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;
using JCommand = OkayegTeaTime.Files.Models.Command;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Code)]
public class CodeCommand : Command
{
    public CodeCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias) : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex pattern = PatternCreator.Create(_alias, _prefix, @"\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            JCommand? command = _twitchBot.CommandController.FindCommand(ChatMessage.LowerSplit[1]);
            if (command is null)
            {
                Response = $"{ChatMessage.Username}, no matching command found";
                return;
            }

            Response = $"{ChatMessage.Username}, https://github.com/Sterbehilfe/OkayegTeaTime/blob/master/OkayegTeaTime.Twitch/Commands/{command.Name}Command.cs";
        }
    }
}
