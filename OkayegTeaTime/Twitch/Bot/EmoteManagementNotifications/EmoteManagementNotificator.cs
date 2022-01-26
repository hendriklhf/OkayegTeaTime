using System.Timers;
using HLE.Collections;
using HLE.Time;
using OkayegTeaTime.Database;
using OkayegTeaTime.Files.JsonClasses.HttpRequests;
using OkayegTeaTime.HttpRequests;
using OkayegTeaTime.Logging;
using OkayegTeaTime.Twitch.Bot.EmoteManagementNotifications.Enums;

namespace OkayegTeaTime.Twitch.Bot.EmoteManagementNotifications;

public class EmoteManagementNotificator
{
    public TwitchBot TwitchBot { get; }

    private readonly List<NotificatorChannel> _channels = new();
    private readonly Timer _timer = new(new Minute().Milliseconds);


    public EmoteManagementNotificator(TwitchBot twitchBot)
    {
        TwitchBot = twitchBot;
        DbController.GetEmoteManagementSubs().ForEach(c => _channels.Add(new(c)));
        InitChannels();
        _timer.Elapsed += Timer_OnElapsed!;
        _timer.Start();
    }

    private void InitChannels()
    {
        _channels.Where(c => AreEmoteListsNull(c)).ForEach(c =>
        {
            List<SevenTvEmote> sevenTvEmotes = HttpRequest.GetSevenTvEmotes(c.Name)?.ToList() ?? new();
            c.New7TVEmotes = sevenTvEmotes;
            c.Old7TVEmotes = sevenTvEmotes;

            List<BttvSharedEmote> bttvEmotes = HttpRequest.GetBttvEmotes(c.Name)?.ToList() ?? new();
            c.NewBTTVEmotes = bttvEmotes;
            c.OldBTTVEmotes = bttvEmotes;

            List<FfzEmote> ffzEmotes = HttpRequest.GetFfzEmotes(c.Name)?.ToList() ?? new();
            c.NewFFZEmotes = ffzEmotes;
            c.OldFFZEmotes = ffzEmotes;
        });
    }

    private void LoadEmotes()
    {
        _channels.ForEach(c =>
        {
            try
            {
                c.Old7TVEmotes = c.New7TVEmotes;
                c.OldBTTVEmotes = c.NewBTTVEmotes;
                c.OldFFZEmotes = c.NewFFZEmotes;

                c.New7TVEmotes = HttpRequest.GetSevenTvEmotes(c.Name)?.ToList();
                c.NewBTTVEmotes = HttpRequest.GetBttvEmotes(c.Name)?.ToList();
                c.NewFFZEmotes = HttpRequest.GetFfzEmotes(c.Name)?.ToList();
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
            if (c.Old7TVEmotes is not null && c.Old7TVEmotes.Count > 0 && c.New7TVEmotes is not null)
            {
                newEmotes = newEmotes.Concat(c.New7TVEmotes.Where(e => c.Old7TVEmotes.Contains(e) == false).Select(e => e.Name)).ToList();
            }
            if (c.OldBTTVEmotes is not null && c.OldBTTVEmotes.Count > 0 && c.NewBTTVEmotes is not null)
            {
                newEmotes = newEmotes.Concat(c.NewBTTVEmotes.Where(e => c.OldBTTVEmotes.Contains(e) == false).Select(e => e.Name)).ToList();
            }
            if (c.OldFFZEmotes is not null & c.OldFFZEmotes!.Count > 0 && c.NewFFZEmotes is not null)
            {
                newEmotes = newEmotes.Concat(c.NewFFZEmotes.Where(e => c.OldFFZEmotes.Contains(e) == false).Select(e => e.Name)).ToList();
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

    private bool AreEmoteListsNull(NotificatorChannel channel)
    {
        return channel.New7TVEmotes is null || channel.Old7TVEmotes is null
            || channel.NewBTTVEmotes is null || channel.OldBTTVEmotes is null
            || channel.NewFFZEmotes is null || channel.OldFFZEmotes is null;
    }

    public void AddChannel(string channel)
    {
        if (!_channels.Any(c => c.Name == channel))
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
