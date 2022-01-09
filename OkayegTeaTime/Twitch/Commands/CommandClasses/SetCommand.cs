using System.Text.RegularExpressions;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands.CommandClasses;

public class SetCommand : Command
{
    public SetCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex prefixPattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\sprefix\s\S+");
        if (prefixPattern.IsMatch(ChatMessage.Message))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendSetPrefix(ChatMessage));
            return;
        }

        Regex emotePattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\semote\s\S+");
        if (emotePattern.IsMatch(ChatMessage.Message))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendSetEmoteInFront(ChatMessage));
            return;
        }

        Regex songRequestPattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix,
            @"\s(sr|songrequests?)\s((1|true|enabled?)|(0|false|disabled?))");
        if (songRequestPattern.IsMatch(ChatMessage.Message))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendSetSongRequestState(ChatMessage));
        }
    }
}
