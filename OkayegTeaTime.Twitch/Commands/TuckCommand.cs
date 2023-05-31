using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE.Emojis;
using HLE.Twitch.Models;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Tuck, typeof(TuckCommand))]
public readonly struct TuckCommand : IChatCommand<TuckCommand>
{
    public ResponseBuilder Response { get; }

    public ChatMessage ChatMessage { get; }

    private readonly TwitchBot _twitchBot;
    private readonly ReadOnlyMemory<char> _prefix;
    private readonly ReadOnlyMemory<char> _alias;

    public TuckCommand(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    {
        ChatMessage = chatMessage;
        Response = new(AppSettings.MaxMessageLength);
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public static void Create(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out TuckCommand command)
    {
        command = new(twitchBot, chatMessage, prefix, alias);
    }

    public ValueTask Handle()
    {
        Regex pattern = _twitchBot.RegexCreator.Create(_alias.Span, _prefix.Span, @"\s\w+(\s\S+)?");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            using ChatMessageExtension messageExtension = new(ChatMessage);
            var target = messageExtension.LowerSplit[1].Span;
            Response.Append(Emoji.PointRight, " ", Emoji.Bed, " ", ChatMessage.Username);
            Response.Append(" tucked ", target, " to bed");
            ReadOnlySpan<char> emote = messageExtension.LowerSplit.Length > 2 ? messageExtension.Split[2].Span : ReadOnlySpan<char>.Empty;
            if (emote.Length > 0)
            {
                Response.Append(" ", emote);
            }
        }

        return ValueTask.CompletedTask;
    }

    public void Dispose()
    {
        Response.Dispose();
    }
}
