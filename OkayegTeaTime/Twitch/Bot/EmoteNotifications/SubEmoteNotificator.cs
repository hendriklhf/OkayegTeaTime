using System.Threading.Tasks;
using HLE.Time;
using OkayegTeaTime.Twitch.Api;
using OkayegTeaTime.Twitch.Bot.EmoteNotifications.Enums;

namespace OkayegTeaTime.Twitch.Bot.EmoteNotifications;

public class SubEmoteNotificator : EmoteNotificator
{
    public override long CheckInterval { get; } = new Hour().Milliseconds;

    private readonly List<SubEmoteNotificatorChannel> _channels = new();

    private const short _taskDelay = 5000;

    public SubEmoteNotificator(TwitchBot twitchBot) : base(twitchBot)
    {
    }

    public override void AddChannel(string channel)
    {
        if (_channels.Any(c => string.Equals(c.Name, channel, StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        InitChannel(channel);
    }

    public override void RemoveChannel(string channel)
    {
        SubEmoteNotificatorChannel? chnl = _channels.FirstOrDefault(c => string.Equals(c.Name, channel, StringComparison.OrdinalIgnoreCase));
        if (chnl is null)
        {
            return;
        }

        _channels.Remove(chnl);
    }

    private protected override void InitChannels()
    {
        /*
        List<string> subChannels = DbController.GetSubEmoteChannels().Select(c => c.ChannelName).ToList();
        foreach (string channel in subChannels)
        {
            _channels.Add(new(channel));
        }
        */

        Task.Run(async () =>
        {
            foreach (SubEmoteNotificatorChannel? c in _channels.Where(AnyEmoteListNull))
            {
                c.OldEmotes = TwitchApi.GetSubEmotes(c.Name);
                await Task.Delay(_taskDelay);
                c.NewEmotes = TwitchApi.GetSubEmotes(c.Name);
                await Task.Delay(_taskDelay);
            }
        }).Wait();
    }

    private protected override void InitChannel(string channel)
    {
        SubEmoteNotificatorChannel chnl = new(channel)
        {
            OldEmotes = TwitchApi.GetSubEmotes(channel),
            NewEmotes = TwitchApi.GetSubEmotes(channel)
        };
        _channels.Add(chnl);
    }

    private protected override void LoadEmotes()
    {
        Task.Run(async () =>
        {
            foreach (SubEmoteNotificatorChannel c in _channels)
            {
                c.OldEmotes = c.NewEmotes;
                c.NewEmotes = TwitchApi.GetSubEmotes(c.Name);
                await Task.Delay(_taskDelay);
            }
        }).Wait();
    }

    private protected override void DetectChange()
    {
        foreach (SubEmoteNotificatorChannel channel in _channels)
        {
            IEnumerable<string>? oldEmoteIds = channel.OldEmotes?.Select(o => o.Id);
            if (oldEmoteIds is null)
            {
                continue;
            }

            List<string>? newEmotes = channel.NewEmotes?.Where(e => !oldEmoteIds.Contains(e.Id)).Select(e => e.Name).ToList();
            NotifyChannel(channel.Name, newEmotes, NotificationType.NewEmote);

            IEnumerable<string>? newEmoteIds = channel.NewEmotes?.Select(n => n.Id);
            if (newEmoteIds is null)
            {
                continue;
            }

            List<string>? removedEmotes = channel.OldEmotes?.Where(e => !newEmoteIds.Contains(e.Id)).Select(e => e.Name).ToList();
            NotifyChannel(channel.Name, removedEmotes, NotificationType.RemovedEmote);
        }
    }

    private protected override void NotifyChannel(string channel, List<string>? emotes, NotificationType type)
    {
        if (emotes is null || !emotes.Any())
        {
            return;
        }

        string message = type switch
        {
            NotificationType.NewEmote => $"Newly added sub emote{(emotes.Count > 1 ? "s" : string.Empty)} in channel #{channel}: {string.Join(" | ", emotes)}",
            NotificationType.RemovedEmote => $"Removed sub emote{(emotes.Count > 1 ? "s" : string.Empty)} in channel #{channel}: {string.Join(" | ", emotes)}",
            _ => $"no message assigned for type {nameof(NotificationType)}.{type}"
        };

        /*
        List<Channel> channels = DbController.GetSubEmoteSubsOf(channel);

        Task.Run(async () =>
        {
            foreach (Channel c in channels)
            {
                TwitchBot.Send(c.ChannelName, message);
                await Task.Delay(AppSettings.DelayBetweenSentMessages);
            }
        }).Wait();
        */
    }

    private bool AnyEmoteListNull(SubEmoteNotificatorChannel channel)
    {
        return channel.OldEmotes is null || channel.NewEmotes is null;
    }
}
