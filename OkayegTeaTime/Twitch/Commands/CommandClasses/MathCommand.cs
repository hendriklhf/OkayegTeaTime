using System.Text.RegularExpressions;
using OkayegTeaTime.HttpRequests;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands.CommandClasses;

public class MathCommand : Command
{
    public MathCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex pattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s.+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Response = $"{ChatMessage.Username}, {HttpRequest.GetMathResult(ChatMessage.Message[(ChatMessage.Split[0].Length + 1)..])}";
        }
    }
}
