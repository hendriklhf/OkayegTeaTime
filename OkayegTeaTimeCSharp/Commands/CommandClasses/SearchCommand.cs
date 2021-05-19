using OkayegTeaTimeCSharp.Twitch;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Commands.CommandClasses
{
    public static class SearchCommand
    {
        private static ChatMessage _chatMessage;

        public static void Handle(TwitchBot twitchBot, ChatMessage chatMessage, string alias)
        {
            _chatMessage = chatMessage;

            if (chatMessage.GetMessage().IsMatch(@"\su(ser(name)?)?:\w+(\s|$)") && chatMessage.GetMessage().IsMatch(@"\sc(hannel)?:#?\w+(\s|$)"))
            {
                twitchBot.SendSearchUserChannel(chatMessage, GetKeyWord(), GetUsername(), GetChannel());
            }
            else if (chatMessage.GetMessage().IsMatch(@"\su(ser(name)?)?:\w+(\s|$)"))
            {
                twitchBot.SendSearchUser(chatMessage, GetKeyWord(), GetUsername());
            }
            else if (chatMessage.GetMessage().IsMatch(@"\sc(hannel)?:#?\w+(\s|$)"))
            {
                twitchBot.SendSearchChannel(chatMessage, GetKeyWord(), GetChannel());
            }
            else if (chatMessage.GetMessage().IsMatch(PatternCreator.CreateBoth(alias, @"\s(\S+(\s|$))+")))
            {
                twitchBot.SendSearch(chatMessage, GetKeyWord());
            }
        }

        private static string GetKeyWord()
        {
            return _chatMessage.GetMessage()[_chatMessage.GetSplit()[0].Length..].ReplacePattern(@"\su(ser(name)?)?:\w+(\s|$)", "").ReplacePattern(@"\sc(hannel)?:#?\w+(\s|$)", "").Trim();
        }

        private static string GetUsername()
        {
            return _chatMessage.GetMessage().Match(@"\su(ser(name)?)?:\w+(\s|$)").Trim().ReplacePattern(@"u(ser(name)?)?:", "");
        }

        private static string GetChannel()
        {
            return _chatMessage.GetMessage().Match(@"\sc(hannel)?:#?\w+(\s|$)").Trim().ReplacePattern(@"c(hannel)?:#?", "");
        }
    }
}