using HLE.Strings;
using OkayegTeaTimeCSharp.Twitch;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Commands.CommandClasses
{
    public static class SearchCommand
    {
        public static void Handle(TwitchBot twitchBot, ChatMessage chatMessage, string alias)
        {
            if (chatMessage.GetMessage().IsMatch(Pattern.SearchUserParameter) && chatMessage.GetMessage().IsMatch(Pattern.SearchChannelParameter))
            {
                twitchBot.Send(chatMessage.Channel, BotActions.SendSearchUserChannel(chatMessage, GetKeyWord(chatMessage), GetUsername(chatMessage), GetChannel(chatMessage)));
            }
            else if (chatMessage.GetMessage().IsMatch(Pattern.SearchUserParameter))
            {
                twitchBot.Send(chatMessage.Channel, BotActions.SendSearchUser(chatMessage, GetKeyWord(chatMessage), GetUsername(chatMessage)));
            }
            else if (chatMessage.GetMessage().IsMatch(Pattern.SearchChannelParameter))
            {
                twitchBot.Send(chatMessage.Channel, BotActions.SendSearchChannel(chatMessage, GetKeyWord(chatMessage), GetChannel(chatMessage)));
            }
            else if (chatMessage.GetMessage().IsMatch(PatternCreator.Create(alias, PrefixHelper.GetPrefix(chatMessage.Channel), @"\s\S+")))
            {
                twitchBot.Send(chatMessage.Channel, BotActions.SendSearch(chatMessage, GetKeyWord(chatMessage)));
            }
        }

        private static string GetKeyWord(ChatMessage chatMessage)
        {
            return chatMessage.GetMessage().Split()[1..].ToSequence().ReplacePattern(Pattern.SearchUserParameter, "").ReplacePattern(Pattern.SearchChannelParameter, "").Trim();
        }

        private static string GetUsername(ChatMessage chatMessage)
        {
            return chatMessage.GetMessage().Match(Pattern.SearchUserParameter).ReplacePattern(@"(-u|--user)", "").Trim();
        }

        private static string GetChannel(ChatMessage chatMessage)
        {
            return chatMessage.GetMessage().Match(Pattern.SearchChannelParameter).ReplacePattern(@"(-c|--channel)", "").Trim();
        }
    }
}