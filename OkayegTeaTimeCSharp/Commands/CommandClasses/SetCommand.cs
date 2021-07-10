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
            if (chatMessage.GetMessage().IsMatch(PatternCreator.Create(alias, PrefixHelper.GetPrefix(chatMessage.Channel), @"\sprefix\s\S{1," + Config.MaxPrefixLength + "}")))
            {
                twitchBot.SendSetPrefix(chatMessage, chatMessage.GetLowerSplit()[2][..(chatMessage.GetLowerSplit()[2].Length > Config.MaxPrefixLength ? Config.MaxPrefixLength : chatMessage.GetLowerSplit()[2].Length)]);
            }
            else if (chatMessage.GetMessage().IsMatch(PatternCreator.Create(alias, PrefixHelper.GetPrefix(chatMessage.Channel), @"\semote\s\S{1," + Config.MaxEmoteInFrontLength + "}")))
            {
                twitchBot.SendSetEmoteInFront(chatMessage, chatMessage.GetSplit()[2][..(chatMessage.GetSplit()[2].Length > Config.MaxEmoteInFrontLength ? Config.MaxEmoteInFrontLength : chatMessage.GetSplit()[2].Length)]);
            }
            else if (chatMessage.GetMessage().IsMatch(PatternCreator.Create(alias, PrefixHelper.GetPrefix(chatMessage.Channel), @"\s(sr|songrequests?)\s((1|true|enabled?)|(0|false|disabled?))")))
            {
                twitchBot.SendSetSongRequestState(chatMessage, ParseBool(chatMessage.GetSplit()[2]));
            }
        }

        private static bool ParseBool(string input)
        {
            return input.IsMatch(@"(1|true|enabled?)");
        }
    }
}