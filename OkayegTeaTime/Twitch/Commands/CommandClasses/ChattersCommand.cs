using HLE.Numbers;
using OkayegTeaTime.HttpRequests;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands.CommandClasses;

public class ChattersCommand : Command
{
    public ChattersCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex pattern = PatternCreator.Create(Alias, Prefix);
        if (pattern.IsMatch(ChatMessage.Message))
        {
            string channel = ChatMessage.LowerSplit.Length > 1 ? ChatMessage.LowerSplit[1] : ChatMessage.Channel;
            DottedNumber chatterCount = HttpRequest.GetChatterCount(channel);

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
}
