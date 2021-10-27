using HLE.Collections;
using HLE.Strings;
using OkayegTeaTimeCSharp.Messages.Interfaces;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;

namespace OkayegTeaTimeCSharp.Commands.CommandClasses;

public class SearchCommand : Command
{
    public SearchCommand(TwitchBot twitchBot, ITwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        if (ChatMessage.Message.IsMatch(Pattern.SearchUserParameter) && ChatMessage.Message.IsMatch(Pattern.SearchChannelParameter))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendSearchUserChannel(ChatMessage, GetKeyWord(), GetUsername(), GetChannel()));
        }
        else if (ChatMessage.Message.IsMatch(Pattern.SearchUserParameter))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendSearchUser(ChatMessage, GetKeyWord(), GetUsername()));
        }
        else if (ChatMessage.Message.IsMatch(Pattern.SearchChannelParameter))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendSearchChannel(ChatMessage, GetKeyWord(), GetChannel()));
        }
        else if (ChatMessage.Message.IsMatch(PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s\S+")))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendSearch(ChatMessage, GetKeyWord()));
        }
    }

    private string GetKeyWord()
    {
        return ChatMessage.Message.Split()[1..].ToSequence().ReplacePattern(Pattern.SearchUserParameter, "").ReplacePattern(Pattern.SearchChannelParameter, "").Trim();
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
