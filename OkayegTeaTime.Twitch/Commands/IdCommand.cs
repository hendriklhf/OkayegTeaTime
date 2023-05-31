using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE.Twitch.Models;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Api.Helix.Models;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Id, typeof(IdCommand))]
public readonly struct IdCommand : IChatCommand<IdCommand>
{
    public ResponseBuilder Response { get; }

    public ChatMessage ChatMessage { get; }

    private readonly TwitchBot _twitchBot;
    private readonly ReadOnlyMemory<char> _prefix;
    private readonly ReadOnlyMemory<char> _alias;

    public IdCommand(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    {
        ChatMessage = chatMessage;
        Response = new(AppSettings.MaxMessageLength);
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public static void Create(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out IdCommand command)
    {
        command = new(twitchBot, chatMessage, prefix, alias);
    }

    public async ValueTask Handle()
    {
        Response.Append(ChatMessage.Username, ", ");
        Regex pattern = _twitchBot.RegexCreator.Create(_alias.Span, _prefix.Span, @"\s\w+");
        long userId;
        if (pattern.IsMatch(ChatMessage.Message))
        {
            using ChatMessageExtension messageExtension = new(ChatMessage);
            ReadOnlyMemory<char> username = messageExtension.LowerSplit[1];
            userId = await GetUserId(username);
            if (userId < 0)
            {
                Response.Append(Messages.TwitchUserDoesntExist);
                return;
            }

            Response.Append(userId);
        }
        else
        {
            userId = ChatMessage.UserId;
            Response.Append("your id: ");
            Response.Append(userId);
        }
    }

    private async ValueTask<long> GetUserId(ReadOnlyMemory<char> username)
    {
        if (username.Span.SequenceEqual(ChatMessage.Username))
        {
            return ChatMessage.UserId;
        }

        User? twitchUser = await _twitchBot.TwitchApi.GetUserAsync(username);
        if (twitchUser is null)
        {
            return -1;
        }

        return twitchUser.Id;
    }

    public void Dispose()
    {
        Response.Dispose();
    }
}
