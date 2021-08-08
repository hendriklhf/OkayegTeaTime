using HLE.Strings;
using OkayegTeaTimeCSharp.Twitch;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Commands.CommandClasses
{
    public static class SetCommand
    {
        public static void Handle(TwitchBot twitchBot, ChatMessage chatMessage, string alias)
        {
            if (chatMessage.GetMessage().IsMatch(PatternCreator.Create(alias, PrefixHelper.GetPrefix(chatMessage.Channel), @"\sprefix\s\S+")))
            {
                twitchBot.Send(chatMessage.Channel, BotActions.SendSetPrefix(chatMessage));
            }
            else if (chatMessage.GetMessage().IsMatch(PatternCreator.Create(alias, PrefixHelper.GetPrefix(chatMessage.Channel), @"\semote\s\S+")))
            {
                twitchBot.Send(chatMessage.Channel, BotActions.SendSetEmoteInFront(chatMessage));
            }
            else if (chatMessage.GetMessage().IsMatch(PatternCreator.Create(alias, PrefixHelper.GetPrefix(chatMessage.Channel), @"\s(sr|songrequests?)\s((1|true|enabled?)|(0|false|disabled?))")))
            {
                twitchBot.Send(chatMessage.Channel, BotActions.SendSetSongRequestState(chatMessage));
            }
        }
    }
}