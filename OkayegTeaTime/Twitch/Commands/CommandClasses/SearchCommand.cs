using HLE.Collections;
using HLE.Strings;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Messages.Interfaces;

namespace OkayegTeaTime.Twitch.Commands.CommandClasses;

public class SearchCommand : Command
{
    public SearchCommand(TwitchBot twitchBot, ITwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        var searchUserPattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, Pattern.SearchUserParameter);
        var searchChannelPattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, Pattern.SearchChannelParameter);

        if (searchUserPattern.IsMatch(ChatMessage.Message))
        {
            if (searchChannelPattern.IsMatch(ChatMessage.Message))
            {
                TwitchBot.Send(ChatMessage.Channel,
                    BotActions.SendSearchUserChannel(ChatMessage, GetKeyWord(), GetUsername(), GetChannel()));
                return;
            }

            TwitchBot.Send(ChatMessage.Channel, BotActions.SendSearchUser(ChatMessage, GetKeyWord(), GetUsername()));
            return;
        }

        if (searchChannelPattern.IsMatch(ChatMessage.Message))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendSearchChannel(ChatMessage, GetKeyWord(), GetChannel()));
        }

        var genericSearchPattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s\S+");
        if (genericSearchPattern.IsMatch(ChatMessage.Message))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendSearch(ChatMessage, GetKeyWord()));
        }
    }

    private string GetKeyWord()
    {
        return ChatMessage.Message.Split()[1..].JoinToString(' ').ReplacePattern(Pattern.SearchUserParameter, "").ReplacePattern(Pattern.SearchChannelParameter, "").Trim();
    }

    private string GetUsername()
    {
        return ChatMessage.Message.Match(Pattern.SearchUserParameter).ReplacePattern(@"(-u|--user)", "").Trim();
    }

    private string GetChannel()
    {
        return ChatMessage.Message.Match(Pattern.SearchChannelParameter).ReplacePattern(@"(-c|--channel)", "").Trim();
    }
}
