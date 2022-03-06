using System.Timers;
using HLE.Collections;
using HLE.Time;
using OkayegTeaTime.Database;
using OkayegTeaTime.Files.JsonClasses.HttpRequests.Bttv;
using OkayegTeaTime.Files.JsonClasses.HttpRequests.Ffz;
using OkayegTeaTime.Files.JsonClasses.HttpRequests.SevenTv;
using OkayegTeaTime.HttpRequests;
using OkayegTeaTime.Logging;
using OkayegTeaTime.Twitch.Bot.EmoteNotifications.Enums;

namespace OkayegTeaTime.Twitch.Bot.EmoteNotifications;

public class ThirdPartyEmoteNotificator
{
    public TwitchBot TwitchBot { get; }

    private readonly List<ThirdPartyNotificatorChannel> _channels = new();
    private readonly long _checkInterval = new Minute(2).Milliseconds;
    private readonly Timer _timer;

    public ThirdPartyEmoteNotificator(TwitchBot twitchBot)
    {
        TwitchBot = twitchBot;
        DbController.GetEmoteManagementSubs().ForEach(c => _channels.Add(new(c)));
        InitChannels();
        _timer = new(_checkInterval);
        _timer.Elapsed += Timer_OnElapsed!;
        _timer.Start();
    }

    private void InitChannels()
    {
        _channels.Where(AreEmoteListsNull).ForEach(c =>
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

    private void LoadEmotes()
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

    private void DetectChange()
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

    private void NotifyChannel(string channel, List<string> emotes, NotificationType type)
    {
        if (type == NotificationType.NewEmote && emotes.Any())
        {
            TwitchBot.Send(channel, $"Newly added emote{(emotes.Count > 1 ? "s" : string.Empty)}: {string.Join(" | ", emotes)}");
        }
    }

    private void Timer_OnElapsed(object sender, ElapsedEventArgs e)
    {
        LoadEmotes();
        DetectChange();
    }

    private bool AreEmoteListsNull(ThirdPartyNotificatorChannel channel)
    {
        return channel.New7TvEmotes is null || channel.Old7TvEmotes is null
            || channel.NewBttvEmotes is null || channel.OldBttvEmotes is null
            || channel.NewFfzEmotes is null || channel.OldFfzEmotes is null;
    }

    public void AddChannel(string channel)
    {
        if (_channels.All(c => c.Name != channel))
        {
            _channels.Add(new(channel));
            InitChannels();
        }
    }

    public void RemoveChannel(string channel)
    {
        _channels.Remove(new(channel));
    }
}
