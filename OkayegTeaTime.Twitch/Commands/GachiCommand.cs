using HLE.Collections;
using HLE.Emojis;
using OkayegTeaTime.Files;
using OkayegTeaTime.Files.Models;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Gachi)]
public sealed class GachiCommand : Command
{
    public GachiCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias) : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        GachiSong? gachi = JsonController.GetGachiSongs().Random();
        if (gachi is null)
        {
            Response = "couldn't find a song";
            return;
        }

        Response = $"{Emoji.PointRight} {gachi.Title} || {gachi.Url} gachiBASS";
    }
}
