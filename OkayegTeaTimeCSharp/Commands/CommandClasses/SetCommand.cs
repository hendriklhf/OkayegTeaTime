using OkayegTeaTimeCSharp.Twitch;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;
using Sterbehilfe.Strings;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Commands.CommandClasses
{
    public static class SetCommand
    {
        public static void Handle(TwitchBot twitchBot, ChatMessage chatMessage, string alias)
        {
            if (chatMessage.GetMessage().IsMatch(PatternCreator.Create(alias, PrefixHelper.GetPrefix(chatMessage.Channel), @"\sprefix\s\S{1,10}")))
            {
                twitchBot.SendSetPrefix(chatMessage, chatMessage.GetLowerSplit()[2][..(chatMessage.GetLowerSplit()[2].Length > 10 ? 10 : chatMessage.GetLowerSplit()[2].Length)]);
            }
        }
    }
}