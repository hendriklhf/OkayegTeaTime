using System.Timers;
using OkayegTeaTime.Twitch.Bot.EmoteNotifications.Enums;

namespace OkayegTeaTime.Twitch.Bot.EmoteNotifications;

public abstract class EmoteNotificator
{
    public abstract long CheckInterval { get; }

    private readonly TwitchBot _twitchBot;
    private readonly Timer _timer;

    protected EmoteNotificator(TwitchBot twitchBot)
    {
        _twitchBot = twitchBot;
        InitChannels();
        _timer = new(CheckInterval);
        _timer.Elapsed += Timer_OnElapsed!;
        _timer.Start();
    }

    public abstract void AddChannel(string channel);

    public abstract void RemoveChannel(string channel);

    private protected abstract void InitChannels();

    private protected abstract void InitChannel(string channel);

    private protected abstract void LoadEmotes();

    private protected abstract void DetectChange();

    private protected virtual void NotifyChannel(string channel, List<string>? emotes, NotificationType type)
    {
        if (emotes is null || !emotes.Any())
        {
            return;
        }

        string message = type switch
        {
            NotificationType.NewEmote => $"Newly added emote{(emotes.Count > 1 ? "s" : string.Empty)}: {string.Join(" | ", emotes)}",
            NotificationType.RemovedEmote => $"Removed emote{(emotes.Count > 1 ? "s" : string.Empty)}: {string.Join(" | ", emotes)}",
            _ => $"no message assigned for type {nameof(NotificationType)}.{type}"
        };

        _twitchBot.Send(channel, message);
    }

    private void Timer_OnElapsed(object sender, ElapsedEventArgs e)
    {
        LoadEmotes();
        DetectChange();
    }
}
