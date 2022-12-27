using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using OkayegTeaTime.Database;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Suggest)]
public readonly unsafe ref struct SuggestCommand
{
    public TwitchChatMessage ChatMessage { get; }

    public Response* Response { get; }

    [SuppressMessage("ReSharper", "NotAccessedField.Local")]
    [SuppressMessage("CodeQuality", "IDE0052:Remove unread private members")]
    private readonly TwitchBot _twitchBot;
    private readonly string? _prefix;
    private readonly string _alias;

    public SuggestCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, Response* response, string? prefix, string alias)
    {
        ChatMessage = chatMessage;
        Response = response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        Regex pattern = PatternCreator.Create(_alias, _prefix, @"\s\S{3,}");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            string suggestion = ChatMessage.Message[(ChatMessage.LowerSplit[0].Length + 1)..];
            DbController.AddSuggestion(ChatMessage.Username, ChatMessage.Channel, suggestion);
            Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, PredefinedMessages.YourSuggestionHasBeenNoted);
        }
    }
}
