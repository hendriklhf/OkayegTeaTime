using HLE.Strings;
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
            if (chatMessage.GetMessage().IsMatch(Pattern.SearchUserParameter) && chatMessage.GetMessage().IsMatch(Pattern.SearchChannelParameter))
            {
                twitchBot.Send(chatMessage.Channel, BotActions.SendSearchUserChannel(chatMessage, GetKeyWord(), GetUsername(), GetChannel()));
            }
            else if (chatMessage.GetMessage().IsMatch(Pattern.SearchUserParameter))
            {
                twitchBot.Send(chatMessage.Channel, BotActions.SendSearchUser(chatMessage, GetKeyWord(), GetUsername()));
            }
            else if (chatMessage.GetMessage().IsMatch(Pattern.SearchChannelParameter))
            {
                twitchBot.Send(chatMessage.Channel, BotActions.SendSearchChannel(chatMessage, GetKeyWord(), GetChannel()));
            }
            else if (chatMessage.GetMessage().IsMatch(PatternCreator.Create(alias, PrefixHelper.GetPrefix(chatMessage.Channel), @"\s\S+")))
            {
                twitchBot.Send(chatMessage.Channel, BotActions.SendSearch(chatMessage, GetKeyWord()));
            }
        }

        private static string GetKeyWord()
        {
            return _chatMessage.GetMessage().Split()[1..].ArrayToString().ReplacePattern(Pattern.SearchUserParameter, "").ReplacePattern(Pattern.SearchChannelParameter, "").Trim();
        }

        private static string GetUsername()
        {
            return _chatMessage.GetMessage().Match(Pattern.SearchUserParameter).ReplacePattern(@"(-u|--user)", "").Trim();
        }

        private static string GetChannel()
        {
            return _chatMessage.GetMessage().Match(Pattern.SearchChannelParameter).ReplacePattern(@"(-c|--channel)", "").Trim();
        }
    }
}