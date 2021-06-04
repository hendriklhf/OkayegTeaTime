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
            if (chatMessage.GetMessage().IsMatch(PatternCreator.CreateBoth(alias, @"\sffz(\s\d+)?")))
            {
                twitchBot.SendFFZEmotes(chatMessage);
            }
            else if (chatMessage.GetMessage().IsMatch(PatternCreator.CreateBoth(alias, @"\sbttv(\s\d+)?")))
            {
                twitchBot.SendBTTVEmotes(chatMessage);
            }
        }
    }
}