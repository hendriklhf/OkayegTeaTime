using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE.Strings;
using HLE.Twitch.Models;
using OkayegTeaTime.Database;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Math, typeof(MathCommand))]
public readonly struct MathCommand(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    : IChatCommand<MathCommand>
{
    public PooledStringBuilder Response { get; } = new(AppSettings.MaxMessageLength);

    public IChatMessage ChatMessage { get; } = chatMessage;

    private readonly TwitchBot _twitchBot = twitchBot;
    private readonly ReadOnlyMemory<char> _prefix = prefix;
    private readonly ReadOnlyMemory<char> _alias = alias;

    public static void Create(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out MathCommand command)
        => command = new(twitchBot, chatMessage, prefix, alias);

    public async ValueTask Handle()
    {
        Regex pattern = _twitchBot.MessageRegexCreator.Create(_alias.Span, _prefix.Span, @"\s.+");
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

    public void Dispose() => Response.Dispose();

    public bool Equals(MathCommand other) =>
        _twitchBot.Equals(other._twitchBot) && _prefix.Equals(other._prefix) && _alias.Equals(other._alias) &&
        Response.Equals(other.Response) && ChatMessage.Equals(other.ChatMessage);

    public override bool Equals(object? obj) => obj is MathCommand other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(_twitchBot, _prefix, _alias, Response, ChatMessage);

    public static bool operator ==(MathCommand left, MathCommand right) => left.Equals(right);

    public static bool operator !=(MathCommand left, MathCommand right) => !left.Equals(right);
}
