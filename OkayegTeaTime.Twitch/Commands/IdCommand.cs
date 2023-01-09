using System;
using System.Text.RegularExpressions;
using HLE;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Id)]
public readonly unsafe ref struct IdCommand
{
    public TwitchChatMessage ChatMessage { get; }

    public StringBuilder* Response { get; }

    private readonly TwitchBot _twitchBot;
    private readonly string? _prefix;
    private readonly string _alias;

    public IdCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        ChatMessage = chatMessage;
        Response = response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace);
        Regex pattern = PatternCreator.Create(_alias, _prefix, @"\s\w+");
        long userId;
        if (pattern.IsMatch(ChatMessage.Message))
        {
            string username = ChatMessage.LowerSplit[1];
            userId = _twitchBot.TwitchApi.GetUserId(username);
            if (userId == -1)
            {
                Response->Append(PredefinedMessages.TwitchUserDoesntExist);
                return;
            }
        }
        else
        {
            userId = ChatMessage.UserId;
        }

        Span<char> userIdChars = stackalloc char[30];
        userId.TryFormat(userIdChars, out int userIdLength);
        userIdChars = userIdChars[..userIdLength];
        Response->Append(userIdChars);
    }
}
