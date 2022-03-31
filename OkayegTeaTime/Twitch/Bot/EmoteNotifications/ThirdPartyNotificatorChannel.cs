using OkayegTeaTime.Files.Jsons.HttpRequests.Bttv;
using OkayegTeaTime.Files.Jsons.HttpRequests.Ffz;
using OkayegTeaTime.Files.Jsons.HttpRequests.SevenTv;

namespace OkayegTeaTime.Twitch.Bot.EmoteNotifications;

public class ThirdPartyNotificatorChannel
{
    public string Name { get; }

    public List<SevenTvEmote>? New7TvEmotes { get; set; }

    public List<SevenTvEmote>? Old7TvEmotes { get; set; }

    public List<BttvSharedEmote>? NewBttvEmotes { get; set; }

    public List<BttvSharedEmote>? OldBttvEmotes { get; set; }

    public List<FfzEmote>? NewFfzEmotes { get; set; }

    public List<FfzEmote>? OldFfzEmotes { get; set; }

    public ThirdPartyNotificatorChannel(string name)
    {
        Name = name;
    }

    public override bool Equals(object? obj)
    {
        return obj is ThirdPartyNotificatorChannel notificatorChannel && string.Equals(notificatorChannel.Name, Name, StringComparison.OrdinalIgnoreCase);
    }
}
