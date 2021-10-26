using HLE.Strings;
using OkayegTeaTimeCSharp.Messages.Interfaces;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;

namespace OkayegTeaTimeCSharp.Commands.CommandClasses;

public class NukeCommand : Command
{
    public NukeCommand(TwitchBot twitchBot, ITwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        if (ChatMessage.Message.IsMatch(PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s\S+\s" + Pattern.TimeSplit + @"\s" + Pattern.TimeSplit + @"(\s|$)")))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendCreatedNuke(ChatMessage));
        }
    }
}
