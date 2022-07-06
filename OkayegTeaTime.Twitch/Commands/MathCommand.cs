using System.Text.RegularExpressions;
using System.Web;
using HLE.Http;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

public class MathCommand : Command
{
    public MathCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex pattern = PatternCreator.Create(_alias, _prefix, @"\s.+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Response = $"{ChatMessage.Username}, {GetMathResult(ChatMessage.Message[(ChatMessage.Split[0].Length + 1)..])}";
        }
    }

    private static string GetMathResult(string expression)
    {
        HttpGet request = new($"https://api.mathjs.org/v4/?expr={HttpUtility.UrlEncode(expression)}");
        return request.Result ?? "api error";
    }
}
