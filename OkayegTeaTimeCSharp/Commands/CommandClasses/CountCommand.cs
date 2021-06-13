using OkayegTeaTimeCSharp.Twitch;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Commands.CommandClasses
{
    public static class CountCommand
    {
        public static void Handle(TwitchBot twitchBot, ChatMessage chatMessage, string alias)
        {
            if (chatMessage.GetMessage().IsMatch(PatternCreator.Create(alias, PrefixHelper.GetPrefix(chatMessage.Channel), @"\s#\w+")))
            {
                twitchBot.SendLoggedMessagesChannelCount(chatMessage, chatMessage.GetLowerSplit()[1]);
            }
            else if (chatMessage.GetMessage().IsMatch(PatternCreator.Create(alias, PrefixHelper.GetPrefix(chatMessage.Channel), @"\s\w+")))
            {
                twitchBot.SendLoggedMessagesUserCount(chatMessage, chatMessage.GetLowerSplit()[1]);
            }
            else if (chatMessage.GetMessage().IsMatch(PatternCreator.Create(alias, PrefixHelper.GetPrefix(chatMessage.Channel))))
            {
                twitchBot.SendLoggedMessagesCount(chatMessage);
            }
        }
    }
}