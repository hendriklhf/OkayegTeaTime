using HLE.Collections;
using HLE.Emojis;
using OkayegTeaTime.Files;
using OkayegTeaTime.Files.Jsons;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

public class GachiCommand : Command
{
    public GachiCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        GachiSong? gachi = JsonController.GachiSongs.Random();
        if (gachi is null)
        {
            Response = $"couldn't find a song";
            return;
        }
        Response = $"{Emoji.PointRight} {gachi.Title} || {gachi.Url} gachiBASS";
        return;
    }
}
