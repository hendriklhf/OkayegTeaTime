using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Messages.Interfaces;

namespace OkayegTeaTime.Twitch.Commands.CommandClasses;

public class BanFromFileCommand : Command
{
    public BanFromFileCommand(TwitchBot twitchBot, ITwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        var pattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s\S+\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            BotActions.SendBanFromFile(TwitchBot, ChatMessage);
        }
    }
}
