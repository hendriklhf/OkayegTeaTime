using HLE.Strings;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;

namespace OkayegTeaTimeCSharp.Commands.CommandClasses
{
    public class SearchCommand : Command
    {
        public SearchCommand(TwitchBot twitchBot, ChatMessage chatMessage, string alias)
            : base(twitchBot, chatMessage, alias)
        {
        }

        public override void Handle()
        {
            if (ChatMessage.GetMessage().IsMatch(Pattern.SearchUserParameter) && ChatMessage.GetMessage().IsMatch(Pattern.SearchChannelParameter))
            {
                TwitchBot.Send(ChatMessage.Channel, BotActions.SendSearchUserChannel(ChatMessage, GetKeyWord(), GetUsername(), GetChannel()));
            }
            else if (ChatMessage.GetMessage().IsMatch(Pattern.SearchUserParameter))
            {
                TwitchBot.Send(ChatMessage.Channel, BotActions.SendSearchUser(ChatMessage, GetKeyWord(), GetUsername()));
            }
            else if (ChatMessage.GetMessage().IsMatch(Pattern.SearchChannelParameter))
            {
                TwitchBot.Send(ChatMessage.Channel, BotActions.SendSearchChannel(ChatMessage, GetKeyWord(), GetChannel()));
            }
            else if (ChatMessage.GetMessage().IsMatch(PatternCreator.Create(Alias, PrefixDictionary.Get(ChatMessage.Channel), @"\s\S+")))
            {
                TwitchBot.Send(ChatMessage.Channel, BotActions.SendSearch(ChatMessage, GetKeyWord()));
            }
        }

        private string GetKeyWord()
        {
            return ChatMessage.GetMessage().Split()[1..].ToSequence().ReplacePattern(Pattern.SearchUserParameter, "").ReplacePattern(Pattern.SearchChannelParameter, "").Trim();
        }

        private string GetUsername()
        {
            return ChatMessage.GetMessage().Match(Pattern.SearchUserParameter).ReplacePattern(@"(-u|--user)", "").Trim();
        }

        private string GetChannel()
        {
            return ChatMessage.GetMessage().Match(Pattern.SearchChannelParameter).ReplacePattern(@"(-c|--channel)", "").Trim();
        }
    }
}
