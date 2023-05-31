using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE.Twitch.Models;
using OkayegTeaTime.Database;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Suggest, typeof(SuggestCommand))]
public readonly struct SuggestCommand : IChatCommand<SuggestCommand>
{
    public ResponseBuilder Response { get; }

    public ChatMessage ChatMessage { get; }

    private readonly TwitchBot _twitchBot;
    private readonly ReadOnlyMemory<char> _prefix;
    private readonly ReadOnlyMemory<char> _alias;

    private SuggestCommand(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    {
        ChatMessage = chatMessage;
        Response = new(AppSettings.MaxMessageLength);
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public static void Create(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out SuggestCommand command)
    {
        command = new(twitchBot, chatMessage, prefix, alias);
    }

    public ValueTask Handle()
    {
        Regex pattern = _twitchBot.RegexCreator.Create(_alias.Span, _prefix.Span, @"\s\S{3,}");
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
}
