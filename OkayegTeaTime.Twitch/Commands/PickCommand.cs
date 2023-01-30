using System;
using System.Text.RegularExpressions;
using HLE;
using HLE.Collections;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Pick)]
public readonly unsafe ref struct PickCommand
{
    public TwitchChatMessage ChatMessage { get; }

    public StringBuilder* Response { get; }

    private readonly TwitchBot _twitchBot;
    private readonly string? _prefix;
    private readonly string _alias;

    public PickCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        ChatMessage = chatMessage;
        Response = response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        Regex pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\s\S+");
        if (!pattern.IsMatch(ChatMessage.Message))
        {
            Response->Append(ChatMessage.Username, Messages.CommaSpace, Messages.NoItemsProvided);
            return;
        }

        Span<string> split = ChatMessage.Split;
        Span<string> pickOptions = split[1..];
        Response->Append(ChatMessage.Username, Messages.CommaSpace, pickOptions.Random());
    }
}
