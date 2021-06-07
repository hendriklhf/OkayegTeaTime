using OkayegTeaTimeCSharp.Twitch;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Commands.CommandClasses
{
    public static class ChattersCommand
    {
        public static void Handle(TwitchBot twitchBot, ChatMessage chatMessage, string alias)
        {
            if (chatMessage.GetMessage().IsMatch(PatternCreator.CreateBoth(alias, PrefixHelper.GetPrefix(chatMessage.Channel), @"\s#?\w+")))
            {
                twitchBot.SendChattersCount(chatMessage, chatMessage.GetLowerSplit()[1]);
            }
            else if (chatMessage.GetMessage().IsMatch(PatternCreator.CreateBoth(alias, PrefixHelper.GetPrefix(chatMessage.Channel))))
            {
                twitchBot.SendChattersCount(chatMessage, chatMessage.Channel);
            }
        }
    }
}