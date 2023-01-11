using System.Text.RegularExpressions;
using HLE;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using StringHelper = OkayegTeaTime.Utils.StringHelper;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Leave)]
public readonly unsafe ref struct LeaveCommand
{
    public TwitchChatMessage ChatMessage { get; }

    public StringBuilder* Response { get; }

    private readonly TwitchBot _twitchBot;
    private readonly string? _prefix;
    private readonly string _alias;

    public LeaveCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
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
            if (!ChatMessage.IsBotModerator)
            {
                Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, PredefinedMessages.YouArentAModeratorOfTheBot);
                return;
            }

            string channel = ChatMessage.LowerSplit[1];
            bool isValidChannel = StringHelper.FormatChannel(ref channel);
            if (!isValidChannel)
            {
                Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, PredefinedMessages.GivenChannelIsInvalid);
                return;
            }

            bool isJoined = _twitchBot.Channels[channel] is not null;
            if (!isJoined)
            {
                Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, "the bot is not connected to #", channel);
                return;
            }

            bool success = _twitchBot.LeaveChannel(channel);
            Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, success ? "successfully left" : "failed to leave", " #", channel);
        }
    }
}
