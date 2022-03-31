using OkayegTeaTime.Twitch.Api;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands.CommandClasses;

public class IdCommand : Command
{
    public IdCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Response = $"{ChatMessage.Username}, ";
        Regex pattern = PatternCreator.Create(Alias, Prefix, @"\s\w+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            string username = ChatMessage.LowerSplit[1];
            string? userId = TwitchApi.GetUserId(username)?.ToString();
            Response += userId ?? PredefinedMessages.TwitchUserDoesntExistMessage;
        }
        else
        {
            Response += ChatMessage.UserId.ToString();
        }
    }
}
