using HLE.Strings;
using OkayegTeaTimeCSharp.Twitch;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Commands.CommandClasses
{
    public class NukeCommand
    {
        public static void Handle(TwitchBot twitchBot, ChatMessage chatMessage, string alias)
        {
            if (chatMessage.GetMessage().IsMatch(PatternCreator.Create(alias, PrefixHelper.GetPrefix(chatMessage.Channel), @"\s\S+\s" + Pattern.TimeSplit + @"\s" + Pattern.TimeSplit + @"(\s|$)")))
            {
                twitchBot.Send(chatMessage.Channel, BotActions.SendCreatedNuke(chatMessage));
            }
        }
    }
}