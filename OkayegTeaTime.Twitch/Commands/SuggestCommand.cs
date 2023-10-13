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

[HandledCommand(CommandType.Suggest, typeof(SuggestCommand))]
public readonly struct SuggestCommand(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    : IChatCommand<SuggestCommand>
{
    public PooledStringBuilder Response { get; } = new(AppSettings.MaxMessageLength);

    public IChatMessage ChatMessage { get; } = chatMessage;

    private readonly TwitchBot _twitchBot = twitchBot;
    private readonly ReadOnlyMemory<char> _prefix = prefix;
    private readonly ReadOnlyMemory<char> _alias = alias;

    public static void Create(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out SuggestCommand command)
    {
        command = new(twitchBot, chatMessage, prefix, alias);
    }

    public ValueTask Handle()
    {
        Regex pattern = _twitchBot.MessageRegexCreator.Create(_alias.Span, _prefix.Span, @"\s\S{3,}");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            using ChatMessageExtension messageExtension = new(ChatMessage);
            string suggestion = ChatMessage.Message[(messageExtension.Split[0].Length + 1)..];
            DbController.AddSuggestion(ChatMessage.Username, ChatMessage.Channel, suggestion);
            Response.Append(ChatMessage.Username, ", ", Messages.YourSuggestionHasBeenNoted);
        }

        return ValueTask.CompletedTask;
    }

    public void Dispose()
    {
        Response.Dispose();
    }

    public bool Equals(SuggestCommand other)
    {
        return _twitchBot.Equals(other._twitchBot) && _prefix.Equals(other._prefix) && _alias.Equals(other._alias) && Response.Equals(other.Response) && ChatMessage.Equals(other.ChatMessage);
    }

    public override bool Equals(object? obj)
    {
        return obj is SuggestCommand other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_twitchBot, _prefix, _alias, Response, ChatMessage);
    }

    public static bool operator ==(SuggestCommand left, SuggestCommand right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(SuggestCommand left, SuggestCommand right)
    {
        return !left.Equals(right);
    }
}
