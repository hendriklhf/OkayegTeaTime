using HLE.Strings;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Twitch.Messages.Interfaces;

namespace OkayegTeaTimeCSharp.Twitch.Commands.CommandClasses;

public class EmoteCommand : Command
{
    public EmoteCommand(TwitchBot twitchBot, ITwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        if (ChatMessage.Message.IsMatch(PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\sffz(\s((\d+)|(\w+(\s\d+)?)))?")))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendFFZEmotes(ChatMessage));
        }
        else if (ChatMessage.Message.IsMatch(PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\sbttv(\s((\d+)|(\w+(\s\d+)?)))?")))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendBTTVEmotes(ChatMessage));
        }
        else if (ChatMessage.Message.IsMatch(PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s7tv(\s((\d+)|(\w+(\s\d+)?)))?")))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.Send7TVEmotes(ChatMessage));
        }
    }
}
