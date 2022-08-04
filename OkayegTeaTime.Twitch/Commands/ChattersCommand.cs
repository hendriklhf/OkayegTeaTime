using System.Text.RegularExpressions;
using HLE.Http;
using HLE.Numbers;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Chatters)]
public class ChattersCommand : Command
{
    public ChattersCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex pattern = PatternCreator.Create(_alias, _prefix);
        if (pattern.IsMatch(ChatMessage.Message))
        {
            string channel = ChatMessage.LowerSplit.Length > 1 ? ChatMessage.LowerSplit[1] : ChatMessage.Channel;
            DottedNumber chatterCount = GetChatterCount(channel);

            switch (chatterCount)
            {
                case > 1:
                    Response = $"{ChatMessage.Username}, there are {chatterCount} chatters in the channel of {channel.Antiping()}";
                    return;
                case > 0:
                    Response = $"{ChatMessage.Username}, there is {chatterCount} chatter in the channel of {channel.Antiping()}";
                    return;
                default:
                    Response = $"{ChatMessage.Username}, there are no chatters in the channel of {channel.Antiping()}";
                    return;
            }
        }
    }

    private static int GetChatterCount(string channel)
    {
        HttpGet request = new($"https://tmi.twitch.tv/group/user/{channel}/chatters");
        return !request.IsValidJsonData ? 0 : request.Data.GetProperty("chatter_count").GetInt32();
    }
}
