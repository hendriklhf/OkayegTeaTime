using OkayegTeaTimeCSharp.Twitch;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;
using Sterbehilfe.Strings;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Commands.CommandClasses
{
    public static class SearchCommand
    {
        private static ChatMessage _chatMessage;

        public static void Handle(TwitchBot twitchBot, ChatMessage chatMessage, string alias)
        {
            _chatMessage = chatMessage;

            if (chatMessage.GetMessage().IsMatch(@"\s?u(ser(name)?)?:\w+") && chatMessage.GetMessage().IsMatch(@"\s?c(hannel)?:#?\w+"))
            {
                twitchBot.SendSearchUserChannel(chatMessage, GetKeyWord(), GetUsername(), GetChannel());
            }
            else if (chatMessage.GetMessage().IsMatch(@"\su(ser(name)?)?:\w+"))
            {
                twitchBot.SendSearchUser(chatMessage, GetKeyWord(), GetUsername());
            }
            else if (chatMessage.GetMessage().IsMatch(@"\sc(hannel)?:#?\w+"))
            {
                twitchBot.SendSearchChannel(chatMessage, GetKeyWord(), GetChannel());
            }
            else if (chatMessage.GetMessage().IsMatch(PatternCreator.Create(alias, PrefixHelper.GetPrefix(chatMessage.Channel), @"\s\S+")))
            {
                twitchBot.SendSearch(chatMessage, GetKeyWord());
            }
        }

        private static string GetKeyWord()
        {
            return _chatMessage.GetMessage()[(_chatMessage.GetSplit()[0].Length + 1)..].ReplacePattern(@"\s?u(ser(name)?)?:\w+", "").ReplacePattern(@"\s?c(hannel)?:#?\w+", "").Trim();
        }

        private static string GetUsername()
        {
            return _chatMessage.GetMessage().Match(@"\s?u(ser(name)?)?:\w+").Trim().ReplacePattern(@"u(ser(name)?)?:", "");
        }

        private static string GetChannel()
        {
            return _chatMessage.GetMessage().Match(@"\s?c(hannel)?:#?\w+").Trim().ReplacePattern(@"c(hannel)?:#?", "");
        }
    }
}