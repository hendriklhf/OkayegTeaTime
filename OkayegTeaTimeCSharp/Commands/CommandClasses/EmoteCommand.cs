using OkayegTeaTimeCSharp.Twitch;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Commands.CommandClasses
{
    public static class EmoteCommand
    {
        public static void Handle(TwitchBot twitchBot, ChatMessage chatMessage, string alias)
        {
            if (chatMessage.GetMessage().IsMatch(PatternCreator.CreateBoth(alias, PrefixHelper.GetPrefix(chatMessage.Channel), @"\sffz(\s\d+)?")))
            {
                twitchBot.SendFFZEmotes(chatMessage);
            }
            else if (chatMessage.GetMessage().IsMatch(PatternCreator.CreateBoth(alias, PrefixHelper.GetPrefix(chatMessage.Channel), @"\sbttv(\s\d+)?")))
            {
                twitchBot.SendBTTVEmotes(chatMessage);
            }
            else if (chatMessage.GetMessage().IsMatch(PatternCreator.CreateBoth(alias, PrefixHelper.GetPrefix(chatMessage.Channel), @"\s7tv(\s\d+)?")))
            {
                twitchBot.Send7TVEmotes(chatMessage);
            }
        }
    }
}