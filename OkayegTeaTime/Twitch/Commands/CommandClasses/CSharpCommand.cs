using OkayegTeaTime.HttpRequests;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands.CommandClasses;

public class CSharpCommand : Command
{
    public CSharpCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex pattern = PatternCreator.Create(Alias, Prefix, @"\s.+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Response = $"{ChatMessage.Username}, {HttpRequest.GetCSharpOnlineCompilerResult(ChatMessage.Message[(ChatMessage.Split[0].Length + 1)..])}";
            return;
        }
    }
}
