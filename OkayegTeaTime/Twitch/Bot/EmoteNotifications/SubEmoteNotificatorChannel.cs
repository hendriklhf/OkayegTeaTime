using TwitchLib.Api.Helix.Models.Chat.Emotes;

namespace OkayegTeaTime.Twitch.Bot.EmoteNotifications;

public class SubEmoteNotificatorChannel
{
    public string Name { get; }

    public ChannelEmote[]? OldEmotes { get; set; }

    public ChannelEmote[]? NewEmotes { get; set; }

    public SubEmoteNotificatorChannel(string name)
    {
        Name = name;
    }
}
