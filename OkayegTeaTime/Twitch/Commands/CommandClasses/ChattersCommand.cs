using System.Text.RegularExpressions;
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
        Regex pattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix);
        if (pattern.IsMatch(ChatMessage.Message))
        {
            string channel = ChatMessage.LowerSplit.Length > 1 ? ChatMessage.LowerSplit[1] : ChatMessage.Channel.Name;
            DottedNumber chatterCount = HttpRequest.GetChatterCount(channel);
            Response = $"{ChatMessage.Username}, ";

            switch (chatterCount)
            {
                case > 1:
                    Response += $"there are {chatterCount} chatters in the channel of {channel}";
                    break;
                case > 0:
                    Response += $"there is {chatterCount} chatter in the channel of {channel}";
                    break;
                default:
                    Response += $"there are no chatters in the channel of {channel}";
                    break;
            }
            return;
        }
    }
}
