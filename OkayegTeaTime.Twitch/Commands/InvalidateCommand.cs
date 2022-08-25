using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Invalidate)]
public class InvalidateCommand : Command
{
    public InvalidateCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        if (!ChatMessage.IsBotModerator)
        {
            Response = $"{ChatMessage.Username}, {PredefinedMessages.NoBotModerator}";
            return;
        }

        _twitchBot.InvalidateCaches();
        Response = $"{ChatMessage.Username}, all database caches have been invalidated";
    }
}
