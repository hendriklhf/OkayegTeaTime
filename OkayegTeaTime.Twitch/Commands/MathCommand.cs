using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Web;
using HLE;
using HLE.Twitch.Models;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Math)]
public readonly unsafe ref struct MathCommand
{
    public ChatMessage ChatMessage { get; }

    public StringBuilder* Response { get; }

    [SuppressMessage("ReSharper", "NotAccessedField.Local")]
    [SuppressMessage("CodeQuality", "IDE0052:Remove unread private members")]
    private readonly TwitchBot _twitchBot;
    private readonly string? _prefix;
    private readonly string _alias;

    public MathCommand(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        ChatMessage = chatMessage;
        Response = response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        Regex pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\s.+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            using ChatMessageExtension messageExtension = new(ChatMessage);
            string mathResult = GetMathResult(ChatMessage.Message[(messageExtension.Split[0].Length + 1)..]);
            Response->Append(ChatMessage.Username, Messages.CommaSpace, mathResult);
        }
    }

    private static string GetMathResult(string expression)
    {
        HttpGet request = new($"https://api.mathjs.org/v4/?expr={HttpUtility.UrlEncode(expression)}");
        return request.Result ?? Messages.ApiError;
    }
}
