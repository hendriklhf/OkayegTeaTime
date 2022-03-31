using OkayegTeaTime.HttpRequests;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands.CommandClasses;

public class CppCommand : Command
{
    public CppCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
#if DEBUG
        Regex pattern = PatternCreator.Create(Alias, Prefix, @"\s.+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            string code = ChatMessage.Message[(ChatMessage.Split[0].Length + 1)..];
            string requestResult = HttpRequest.GetCppOnlineCompilerResult(code);
            Response = $"{ChatMessage.Username}, {requestResult}";
        }
#endif
    }
}
