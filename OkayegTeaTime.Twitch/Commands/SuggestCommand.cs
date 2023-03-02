using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using HLE.Twitch;
using HLE.Twitch.Models;
using OkayegTeaTime.Database;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Suggest)]
public readonly ref struct SuggestCommand
{
    public ChatMessage ChatMessage { get; }

    private readonly ref MessageBuilder _response;

    [SuppressMessage("ReSharper", "NotAccessedField.Local")]
    [SuppressMessage("CodeQuality", "IDE0052:Remove unread private members")]
    private readonly TwitchBot _twitchBot;
    private readonly ReadOnlySpan<char> _prefix;
    private readonly ReadOnlySpan<char> _alias;

    public SuggestCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        ChatMessage = chatMessage;
        _response = ref response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        Regex pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\s\S{3,}");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            using ChatMessageExtension messageExtension = new(ChatMessage);
            string suggestion = ChatMessage.Message[(messageExtension.Split[0].Length + 1)..];
            DbController.AddSuggestion(ChatMessage.Username, ChatMessage.Channel, suggestion);
            _response.Append(ChatMessage.Username, ", ", Messages.YourSuggestionHasBeenNoted);
        }
    }
}
