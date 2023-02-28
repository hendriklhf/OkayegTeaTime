using System.Text.RegularExpressions;
using HLE;
using HLE.Twitch.Models;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using StringHelper = OkayegTeaTime.Utils.StringHelper;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Join)]
public readonly unsafe ref struct JoinCommand
{
    public ChatMessage ChatMessage { get; }

    public StringBuilder* Response { get; }

    private readonly TwitchBot _twitchBot;
    private readonly string? _prefix;
    private readonly string _alias;

    public JoinCommand(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        ChatMessage = chatMessage;
        Response = response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        Regex pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\s#?\w{3,25}");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            using ChatMessageExtension messageExtension = new(ChatMessage);
            if (!messageExtension.IsBotModerator)
            {
                Response->Append(ChatMessage.Username, Messages.CommaSpace, Messages.YouArentAModeratorOfTheBot);
                return;
            }

            string channel = new(messageExtension.LowerSplit[1]);
            bool isValidChannel = StringHelper.FormatChannel(ref channel);
            if (!isValidChannel)
            {
                Response->Append(ChatMessage.Username, Messages.CommaSpace, Messages.GivenChannelIsInvalid);
                return;
            }

            bool isJoined = _twitchBot.Channels[channel] is not null;
            if (isJoined)
            {
                Response->Append(ChatMessage.Username, Messages.CommaSpace, "the bot is already connected to #", channel);
                return;
            }

            bool channelExists = _twitchBot.TwitchApi.DoesUserExist(channel);
            if (!channelExists)
            {
                Response->Append(ChatMessage.Username, Messages.CommaSpace, Messages.GivenChannelDoesNotExist);
                return;
            }

            bool success = _twitchBot.JoinChannel(channel);
            Response->Append(ChatMessage.Username, Messages.CommaSpace, success ? "successfully joined" : "failed to join", " #", channel);
        }
    }
}
