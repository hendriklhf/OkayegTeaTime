using System.Text.RegularExpressions;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

public class IdCommand : Command
{
    public IdCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Response = $"{ChatMessage.Username}, ";
        Regex pattern = PatternCreator.Create(_alias, _prefix, @"\s\w+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            string username = ChatMessage.LowerSplit[1];
            string? userId = _twitchBot.TwitchApi.GetUserId(username)?.ToString();
            Response += userId ?? PredefinedMessages.TwitchUserDoesntExistMessage;
        }
        else
        {
            Response += ChatMessage.UserId.ToString();
        }
    }
}
