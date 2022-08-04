using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Chatterino)]
public class ChatterinoCommand : Command
{
    public ChatterinoCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Response = "Website: chatterino.com || Releases: github.com/Chatterino/chatterino2/releases";
    }
}
