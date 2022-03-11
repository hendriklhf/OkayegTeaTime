using HLE.Collections;
using HLE.Time;
using OkayegTeaTime.Files.JsonClasses.HttpRequests.Bttv;
using OkayegTeaTime.Files.JsonClasses.HttpRequests.Ffz;
using OkayegTeaTime.Files.JsonClasses.HttpRequests.SevenTv;
using OkayegTeaTime.HttpRequests;
using OkayegTeaTime.Logging;
using OkayegTeaTime.Twitch.Bot.EmoteNotifications.Enums;

namespace OkayegTeaTime.Twitch.Bot.EmoteNotifications;

public class ThirdPartyEmoteNotificator : EmoteNotificator
{
    public override long CheckInterval { get; } = new Minute(2).Milliseconds;

    private readonly List<ThirdPartyNotificatorChannel> _channels = new();

    public ThirdPartyEmoteNotificator(TwitchBot twitchBot) : base(twitchBot)
    {
    }

    public override void AddChannel(string channel)
    {
        if (!_channels.All(c => string.Equals(channel, c.Name, StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        InitChannel(channel);
    }

    public override void RemoveChannel(string channel)
    {
        ThirdPartyNotificatorChannel? chnl = _channels.FirstOrDefault(c => string.Equals(channel, c.Name, StringComparison.OrdinalIgnoreCase));
        if (chnl is null)
        {
            return;
        }

        _channels.Remove(chnl);
    }

    private protected override void InitChannels()
    {
        _channels.Where(AnyEmoteListNull).ForEach(c =>
        {
            List<SevenTvEmote> sevenTvEmotes = HttpRequest.GetSevenTvEmotes(c.Name)?.ToList() ?? new();
            c.New7TvEmotes = sevenTvEmotes;
            c.Old7TvEmotes = sevenTvEmotes;

            List<BttvSharedEmote> bttvEmotes = HttpRequest.GetBttvEmotes(c.Name)?.ToList() ?? new();
            c.NewBttvEmotes = bttvEmotes;
            c.OldBttvEmotes = bttvEmotes;

            List<FfzEmote> ffzEmotes = HttpRequest.GetFfzEmotes(c.Name)?.ToList() ?? new();
            c.NewFfzEmotes = ffzEmotes;
            c.OldFfzEmotes = ffzEmotes;
        });
    }

    private protected override void InitChannel(string channel)
    {
        List<SevenTvEmote> sevenTvEmotes = HttpRequest.GetSevenTvEmotes(channel)?.ToList() ?? new();
        List<BttvSharedEmote> bttvEmotes = HttpRequest.GetBttvEmotes(channel)?.ToList() ?? new();
        List<FfzEmote> ffzEmotes = HttpRequest.GetFfzEmotes(channel)?.ToList() ?? new();

        ThirdPartyNotificatorChannel chnl = new(channel)
        {
            Old7TvEmotes = sevenTvEmotes,
            New7TvEmotes = sevenTvEmotes,
            OldBttvEmotes = bttvEmotes,
            NewBttvEmotes = bttvEmotes,
            OldFfzEmotes = ffzEmotes,
            NewFfzEmotes = ffzEmotes
        };

        _channels.Add(chnl);
    }

    private protected override void LoadEmotes()
    {
        _channels.ForEach(c =>
        {
            try
            {
                c.Old7TvEmotes = c.New7TvEmotes;
                c.OldBttvEmotes = c.NewBttvEmotes;
                c.OldFfzEmotes = c.NewFfzEmotes;

                c.New7TvEmotes = HttpRequest.GetSevenTvEmotes(c.Name)?.ToList();
                c.NewBttvEmotes = HttpRequest.GetBttvEmotes(c.Name)?.ToList();
                c.NewFfzEmotes = HttpRequest.GetFfzEmotes(c.Name)?.ToList();
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        });
    }

    private protected override void DetectChange()
    {
        _channels.ForEach(c =>
        {
            List<string> newEmotes = new();
            if (c.Old7TvEmotes is not null && c.Old7TvEmotes.Count > 0 && c.New7TvEmotes is not null)
            {
                newEmotes = newEmotes.Concat(c.New7TvEmotes.Where(e => c.Old7TvEmotes.Contains(e) == false).Select(e => e.Name)).ToList();
            }

            if (c.OldBttvEmotes is not null && c.OldBttvEmotes.Count > 0 && c.NewBttvEmotes is not null)
            {
                newEmotes = newEmotes.Concat(c.NewBttvEmotes.Where(e => c.OldBttvEmotes.Contains(e) == false).Select(e => e.Name)).ToList();
            }

            if (c.OldFfzEmotes is not null & c.OldFfzEmotes!.Count > 0 && c.NewFfzEmotes is not null)
            {
                newEmotes = newEmotes.Concat(c.NewFfzEmotes.Where(e => c.OldFfzEmotes.Contains(e) == false).Select(e => e.Name)).ToList();
            }

            NotifyChannel(c.Name, newEmotes, NotificationType.NewEmote);
        });
    }

    private bool AnyEmoteListNull(ThirdPartyNotificatorChannel channel)
    {
        return channel.New7TvEmotes is null || channel.Old7TvEmotes is null
            || channel.NewBttvEmotes is null || channel.OldBttvEmotes is null
            || channel.NewFfzEmotes is null || channel.OldFfzEmotes is null;
    }
}
