using HLE.Strings;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Twitch.Messages.Interfaces;
using OkayegTeaTimeCSharp.Utils;

namespace OkayegTeaTimeCSharp.Twitch.Commands.CommandClasses;

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
