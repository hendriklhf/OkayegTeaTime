using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using HLE.Twitch.Models;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Math, typeof(MathCommand))]
public readonly struct MathCommand : IChatCommand<MathCommand>
{
    public ResponseBuilder Response { get; }

    public ChatMessage ChatMessage { get; }

    private readonly TwitchBot _twitchBot;
    private readonly ReadOnlyMemory<char> _prefix;
    private readonly ReadOnlyMemory<char> _alias;

    public MathCommand(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    {
        ChatMessage = chatMessage;
        Response = new(AppSettings.MaxMessageLength);
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public static void Create(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out MathCommand command)
    {
        command = new(twitchBot, chatMessage, prefix, alias);
    }

    public async ValueTask Handle()
    {
        Regex pattern = _twitchBot.RegexCreator.Create(_alias.Span, _prefix.Span, @"\s.+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            using ChatMessageExtension messageExtension = new(ChatMessage);
            string? mathResult = await GetMathResult(ChatMessage.Message[(messageExtension.Split[0].Length + 1)..]);
            Response.Append(ChatMessage.Username, ", ", mathResult);
        }
    }

    private static async ValueTask<string?> GetMathResult(string expression)
    {
        HttpGet request = await HttpGet.GetStringAsync($"https://api.mathjs.org/v4/?expr={HttpUtility.UrlEncode(expression)}");
        return request.Result;
    }

    public void Dispose()
    {
        Response.Dispose();
    }
}
