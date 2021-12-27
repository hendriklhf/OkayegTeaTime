using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Messages.Interfaces;

namespace OkayegTeaTime.Twitch.Commands.CommandClasses;

public class SuggestCommand : Command
{
    public SuggestCommand(TwitchBot twitchBot, ITwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        var pattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s\S{3,}");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendSuggestionNoted(ChatMessage));
        }
    }
}
