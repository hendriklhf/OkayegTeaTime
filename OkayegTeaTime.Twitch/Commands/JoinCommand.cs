using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE.Twitch.Models;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Api.Helix.Models;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using StringHelper = OkayegTeaTime.Utils.StringHelper;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Join, typeof(JoinCommand))]
public readonly struct JoinCommand : IChatCommand<JoinCommand>
{
    public ResponseBuilder Response { get; }

    public ChatMessage ChatMessage { get; }

    private readonly TwitchBot _twitchBot;
    private readonly ReadOnlyMemory<char> _prefix;
    private readonly ReadOnlyMemory<char> _alias;

    public JoinCommand(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    {
        ChatMessage = chatMessage;
        Response = new(AppSettings.MaxMessageLength);
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public static void Create(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out JoinCommand command)
    {
        command = new(twitchBot, chatMessage, prefix, alias);
    }

    public async ValueTask Handle()
    {
        Regex pattern = _twitchBot.RegexCreator.Create(_alias.Span, _prefix.Span, @"\s#?\w{3,25}");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            using ChatMessageExtension messageExtension = new(ChatMessage);
            if (!messageExtension.IsBotModerator)
            {
                Response.Append(ChatMessage.Username, ", ", Messages.YouArentAModeratorOfTheBot);
                return;
            }

            string channel = new(messageExtension.LowerSplit[1].Span);
            bool isValidChannel = StringHelper.FormatChannel(ref channel);
            if (!isValidChannel)
            {
                Response.Append(ChatMessage.Username, ", ", Messages.GivenChannelIsInvalid);
                return;
            }

            bool isJoined = _twitchBot.Channels[channel] is not null;
            if (isJoined)
            {
                Response.Append(ChatMessage.Username, ", ", "the bot is already connected to #", channel);
                return;
            }

            User? user = await _twitchBot.TwitchApi.GetUserAsync(channel);
            if (user is null)
            {
                Response.Append(ChatMessage.Username, ", ", Messages.GivenChannelDoesNotExist);
                return;
            }

            bool success = await _twitchBot.JoinChannel(channel);
            Response.Append(ChatMessage.Username, ", ", success ? "successfully joined" : "failed to join", " #", channel);
        }
    }

    public void Dispose()
    {
        Response.Dispose();
    }
}
