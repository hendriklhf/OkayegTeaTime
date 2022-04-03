using HLE.Collections;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

public class PickCommand : Command
{
    public PickCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Response = $"{ChatMessage.Username}, {ChatMessage.Split[1..].Random()}";
    }
}
