using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE.Twitch.Models;
using OkayegTeaTime.Database;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

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
            Response.Append(ChatMessage.Username, ", ");
            using ChatMessageExtension messageExtension = new(ChatMessage);
            string expression = ChatMessage.Message[(messageExtension.Split[0].Length + 1)..];
            try
            {
                string result = await _twitchBot.MathService.GetExpressionResultAsync(expression);
                Response.Append(result);
            }
            catch (Exception ex)
            {
                await DbController.LogExceptionAsync(ex);
                Response.Append(Messages.ApiError);
            }
        }
    }

    public void Dispose()
    {
        Response.Dispose();
    }
}
