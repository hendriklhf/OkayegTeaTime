using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Chatterino7)]
public class Chatterino7Command : Command
{
    public Chatterino7Command(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
        Response = $"Website: 7tv.app || Releases: github.com/SevenTV/chatterino7/releases";
    }

    public override void Handle()
    {
        Response = $"Website: 7tv.app || Releases: github.com/SevenTV/chatterino7/releases";
    }
}
