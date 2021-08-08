using HLE.Strings;
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
            if (chatMessage.GetMessage().IsMatch(PatternCreator.Create(alias, PrefixHelper.GetPrefix(chatMessage.Channel), @"\sffz(\s((\d+)|(\w+(\s\d+)?)))?")))
            {
                twitchBot.Send(chatMessage.Channel, BotActions.SendFFZEmotes(chatMessage));
            }
            else if (chatMessage.GetMessage().IsMatch(PatternCreator.Create(alias, PrefixHelper.GetPrefix(chatMessage.Channel), @"\sbttv(\s((\d+)|(\w+(\s\d+)?)))?")))
            {
                twitchBot.Send(chatMessage.Channel, BotActions.SendBTTVEmotes(chatMessage));
            }
            else if (chatMessage.GetMessage().IsMatch(PatternCreator.Create(alias, PrefixHelper.GetPrefix(chatMessage.Channel), @"\s7tv(\s((\d+)|(\w+(\s\d+)?)))?")))
            {
                twitchBot.Send(chatMessage.Channel, BotActions.Send7TVEmotes(chatMessage));
            }
        }
    }
}