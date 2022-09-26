using System.Text.RegularExpressions;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Join)]
public sealed class JoinCommand : Command
{
    public JoinCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias) : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex pattern = PatternCreator.Create(_alias, _prefix, @"\s#?\w{3,25}");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            if (!ChatMessage.IsBotModerator)
            {
                Response = $"{ChatMessage.Username}, {PredefinedMessages.NoBotModerator}";
                return;
            }

            string channel = ChatMessage.LowerSplit[1];
            bool isValidChannel = StringHelper.FormatChannel(ref channel);
            if (!isValidChannel)
            {
                Response = $"{ChatMessage.Username}, the given channel is invalid";
                return;
            }

            bool isJoined = _twitchBot.Channels[channel] is not null;
            if (isJoined)
            {
                Response = $"{ChatMessage.Username}, the bot is already connected to #{channel}";
                return;
            }

            bool channelExists = _twitchBot.TwitchApi.DoesUserExist(channel);
            if (!channelExists)
            {
                Response = $"{ChatMessage.Username}, the given channel does not exist";
                return;
            }

            bool success = _twitchBot.JoinChannel(channel);
            Response = $"{ChatMessage.Username}, {(success ? "successfully joined" : "failed to join")} #{channel}";
        }
    }
}
