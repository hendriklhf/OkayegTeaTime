using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Messages.Interfaces;

namespace OkayegTeaTime.Twitch.Commands.CommandClasses;

public class SetCommand : Command
{
    public SetCommand(TwitchBot twitchBot, ITwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        var prefixPattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\sprefix\s\S+");
        if (prefixPattern.IsMatch(ChatMessage.Message))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendSetPrefix(ChatMessage));
            return;
        }

        var emotePattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\semote\s\S+");
        if (emotePattern.IsMatch(ChatMessage.Message))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendSetEmoteInFront(ChatMessage));
            return;
        }

        var songRequestPattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix,
            @"\s(sr|songrequests?)\s((1|true|enabled?)|(0|false|disabled?))");
        if (songRequestPattern.IsMatch(ChatMessage.Message))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendSetSongRequestState(ChatMessage));
        }
    }
}
