using System.Text.RegularExpressions;
using HLE;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

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
        Response->Append(ChatMessage.Username, Messages.CommaSpace);
        Regex pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\s\w+");
        long userId;
        if (pattern.IsMatch(ChatMessage.Message))
        {
            string username = ChatMessage.LowerSplit[1];
            userId = _twitchBot.TwitchApi.GetUserId(username);
            if (userId == -1)
            {
                Response->Append(Messages.TwitchUserDoesntExist);
                return;
            }
        }
        else
        {
            userId = ChatMessage.UserId;
        }

        Response->Append(userId);
    }
}
