using HLE.Strings;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Messages.Interfaces;

namespace OkayegTeaTime.Twitch.Commands.CommandClasses;

public class FfzCommand : Command
{
    public FfzCommand(TwitchBot twitchBot, ITwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        var channelWithCountPattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s\w+\s\d+");
        if (channelWithCountPattern.IsMatch(ChatMessage.Message))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendFfzEmotes(ChatMessage, ChatMessage.Split[1], ChatMessage.Split[2].ToInt()));
            return;
        }

        var channelPattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s\w+");
        if (channelPattern.IsMatch(ChatMessage.Message))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendFfzEmotes(ChatMessage, ChatMessage.Split[1]));
            return;
        }

        var countPattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s\d+");
        if (countPattern.IsMatch(ChatMessage.Message))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendFfzEmotes(ChatMessage, count: ChatMessage.Split[1].ToInt()));
            return;
        }

        TwitchBot.Send(ChatMessage.Channel, BotActions.SendFfzEmotes(ChatMessage));
    }
}
