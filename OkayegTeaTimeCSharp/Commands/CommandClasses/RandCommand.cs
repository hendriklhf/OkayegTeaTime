using OkayegTeaTimeCSharp.Twitch;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Commands.CommandClasses
{
    public static class RandCommand
    {
        public static void Handle(TwitchBot twitchBot, ChatMessage chatMessage, string alias)
        {
            if (chatMessage.GetMessage().IsMatch(PatternCreator.CreateBoth(alias, PrefixHelper.GetPrefix(chatMessage.Channel), @"\s\w+\s#?\w+")))
            {
                twitchBot.SendRandomMessage(chatMessage, chatMessage.GetLowerSplit()[1], chatMessage.GetLowerSplit()[2].ReplaceHashtag());
            }
            else if (chatMessage.GetMessage().IsMatch(PatternCreator.CreateBoth(alias, PrefixHelper.GetPrefix(chatMessage.Channel), @"\s\w+")))
            {
                twitchBot.SendRandomMessage(chatMessage, chatMessage.GetLowerSplit()[1]);
            }
            else if (chatMessage.GetMessage().IsMatch(PatternCreator.CreateBoth(alias, PrefixHelper.GetPrefix(chatMessage.Channel))))
            {
                twitchBot.SendRandomMessage(chatMessage);
            }
        }
    }
}