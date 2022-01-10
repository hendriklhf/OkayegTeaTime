using HLE.Emojis;
using HLE.Strings;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands.CommandClasses;

public class GachiCommand : Command
{
    public GachiCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Gachi? gachi = DbController.GetRandomGachi();
        if (gachi is null)
        {
            Response = $"couldn't find a song";
            return;
        }
        Response = $"{Emoji.PointRight} {gachi.Title.Decode()} || {gachi.Link} gachiBASS";
    }
}
