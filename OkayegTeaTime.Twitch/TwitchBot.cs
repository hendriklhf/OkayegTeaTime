using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using HLE.Collections;
using HLE.Emojis;
using HLE.Strings;
using HLE.Twitch;
using HLE.Twitch.Models;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Cache;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Api.Bttv;
using OkayegTeaTime.Twitch.Api.Ffz;
using OkayegTeaTime.Twitch.Api.Helix;
using OkayegTeaTime.Twitch.Api.SevenTv;
using OkayegTeaTime.Twitch.Controller;
using OkayegTeaTime.Twitch.Handlers;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;
using static OkayegTeaTime.Utils.ProcessUtils;

namespace OkayegTeaTime.Twitch;

public sealed class TwitchBot
{
    public UserCache Users { get; } = new();

    public ReminderCache Reminders { get; } = new();

    public ChannelCache Channels { get; } = new();

    public SpotifyUserCache SpotifyUsers { get; } = new();

    public CommandController CommandController { get; } = new();

    public WeatherController WeatherController { get; } = new();

    public CooldownController CooldownController { get; }

    public TwitchApi TwitchApi { get; } = new(AppSettings.Twitch.ApiClientId, AppSettings.Twitch.ApiClientSecret, new());

    public FfzApi FfzApi { get; } = new(new());

    public BttvApi BttvApi { get; } = new(new());

    public SevenTvApi SevenTvApi { get; } = new(new());

    public RegexCreator RegexCreator { get; set; } = new();

    public LastMessageController LastMessages { get; }

    public Dictionary<long, HangmanGame> HangmanGames { get; } = new();

    public uint CommandCount { get; set; }

    private readonly TwitchClient _twitchClient;
    private readonly MessageHandler _messageHandler;
    private readonly TimerCollection _timerCollection = new();

    public TwitchBot(IEnumerable<string>? channels = null)
    {
        _twitchClient = new(AppSettings.Twitch.Username, AppSettings.Twitch.OAuthToken, new()
        {
            UseSSL = true
        });

        channels ??= Channels.Select(c => c.Name);
        _twitchClient.JoinChannelsAsync(channels.ToArray()).AsTask().Wait(); // TODO: 💢

        _twitchClient.OnConnected += Client_OnConnected!;
        _twitchClient.OnJoinedChannel += async (_, e) => await Client_OnJoinedChannel(e);
        _twitchClient.OnChatMessageReceived += async (_, msg) => await Client_OnMessageReceived(msg);
        _twitchClient.OnDisconnected += Client_OnDisconnect!;

        CooldownController = new(CommandController);
        LastMessages = new(Channels);
        _messageHandler = new(this);
        InitializeTimers();
    }

    public async ValueTask ConnectAsync()
    {
        await _twitchClient.ConnectAsync();
        _timerCollection.StartAll();
    }

    public async ValueTask DisconnectAsync()
    {
        _timerCollection.StopAll();
        await _twitchClient.DisconnectAsync();
    }

    public async ValueTask SendAsync(long channelId, ReadOnlyMemory<char> message)
    {
        await _twitchClient.SendAsync(channelId, message);
    }

    public async ValueTask SendAsync(string channel, string message, bool addEmote = true, bool checkLength = true, bool checkDuplicate = true)
    {
        if (addEmote)
        {
            string emote = Channels[channel]?.Emote ?? AppSettings.DefaultEmote;
            message = emote + ' ' + message;
        }

        if (checkDuplicate && message == LastMessages[channel])
        {
            message = message + ' ' + AppSettings.ChatterinoChar;
        }

        if (checkLength && message.Length > AppSettings.MaxMessageLength)
        {
            message = message[..AppSettings.MaxMessageLength];
        }

        await _twitchClient.SendAsync(_twitchClient.Channels[channel]!.Id, message);
        LastMessages[channel] = message;
    }

    public async ValueTask<bool> JoinChannel(string channel)
    {
        try
        {
            await _twitchClient.JoinChannelAsync(channel);
            await SendAsync(channel, $"{Emoji.Wave} hello");
            var user = await TwitchApi.GetUserAsync(channel);
            if (user is null)
            {
                return false;
            }

            Channels.Add(user.Id, channel);
            return true;
        }
        catch (Exception ex)
        {
            await DbController.LogExceptionAsync(ex);
            return false;
        }
    }

    public async ValueTask<bool> LeaveChannel(string channel)
    {
        try
        {
            await SendAsync(channel, $"{Emoji.Wave} bye");
            await _twitchClient.LeaveChannelAsync(channel);
            Channels.Remove(channel);
            return true;
        }
        catch (Exception ex)
        {
            await DbController.LogExceptionAsync(ex);
            return false;
        }
    }

    public void InvalidateCaches()
    {
        Reminders.Invalidate();
        SpotifyUsers.Invalidate();
        Users.Invalidate();
        Channels.Invalidate();
    }

    public async ValueTask<string> GetBestEmoteAsync(long channelId, string fallback, params string[] keywords)
    {
        using PoolBufferList<string> emoteNames = await GetAllEmoteNames(channelId);
        using PoolBufferList<string> bestEmotes = new(32);
        foreach (string keyword in keywords)
        {
            Regex keywordPattern = RegexPool.Shared.GetOrAdd(keyword, RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            for (int i = 0; i < emoteNames.Count; i++)
            {
                string emoteName = emoteNames[i];
                if (keywordPattern.IsMatch(emoteName))
                {
                    bestEmotes.Add(emoteName);
                }
            }
        }

        return bestEmotes.AsSpan().Random() ?? fallback;
    }

    public async ValueTask<PoolBufferList<string>> GetAllEmoteNames(long channelId)
    {
        var getTwitchGlobalEmotesTask = TwitchApi.GetGlobalEmotesAsync().AsTask();
        var getTwitchChannelEmotesTask = TwitchApi.GetChannelEmotesAsync(channelId).AsTask();
        var getFfzGlobalEmotesTask = FfzApi.GetGlobalEmotesAsync().AsTask();
        var getFfzChannelEmotesTask = FfzApi.GetChannelEmotesAsync(channelId).AsTask();
        var getBttvGlobalEmotesTask = BttvApi.GetGlobalEmotesAsync().AsTask();
        var getBttvChannelEmotesTask = BttvApi.GetChannelEmotesAsync(channelId).AsTask();
        var getSevenTvGlobalEmotesTask = SevenTvApi.GetGlobalEmotesAsync().AsTask();
        var getSevenTvChannelEmotesTask = SevenTvApi.GetChannelEmotesAsync(channelId).AsTask();

        await Task.WhenAll(getTwitchGlobalEmotesTask, getTwitchChannelEmotesTask, getFfzGlobalEmotesTask, getFfzChannelEmotesTask,
            getBttvGlobalEmotesTask, getBttvChannelEmotesTask, getSevenTvGlobalEmotesTask, getSevenTvChannelEmotesTask);

        var twitchGlobalEmotes = getTwitchGlobalEmotesTask.Result;
        var twitchChannelEmotes = getTwitchChannelEmotesTask.Result;
        var ffzGlobalEmotes = getFfzGlobalEmotesTask.Result;
        var ffzChannelEmotes = getFfzChannelEmotesTask.Result;
        var bttvGlobalEmotes = getBttvGlobalEmotesTask.Result;
        var bttvChannelEmotes = getBttvChannelEmotesTask.Result;
        var sevenTvGlobalEmotes = getSevenTvGlobalEmotesTask.Result;
        var sevenTvChannelEmotes = getSevenTvChannelEmotesTask.Result;

        int totalEmoteCount = twitchGlobalEmotes.Length + twitchChannelEmotes.Length + ffzGlobalEmotes.Length + (ffzChannelEmotes?.Length ?? 0) +
                              bttvGlobalEmotes.Length + (bttvChannelEmotes?.Length ?? 0) + sevenTvGlobalEmotes.Length + (sevenTvChannelEmotes?.Length ?? 0);

        PoolBufferList<string> emoteNames = new(totalEmoteCount);

        foreach (var emote in twitchGlobalEmotes)
        {
            emoteNames.Add(emote.Name);
        }

        foreach (var emote in twitchChannelEmotes)
        {
            emoteNames.Add(emote.Name);
        }

        foreach (var emote in ffzGlobalEmotes)
        {
            emoteNames.Add(emote.Name);
        }

        if (ffzChannelEmotes is not null)
        {
            foreach (var emote in ffzChannelEmotes)
            {
                emoteNames.Add(emote.Name);
            }
        }

        foreach (var emote in bttvGlobalEmotes)
        {
            emoteNames.Add(emote.Name);
        }

        if (bttvChannelEmotes is not null)
        {
            foreach (var emote in bttvChannelEmotes)
            {
                emoteNames.Add(emote.Name);
            }
        }

        foreach (var emote in sevenTvGlobalEmotes)
        {
            emoteNames.Add(emote.Name);
        }

        if (sevenTvChannelEmotes is not null)
        {
            foreach (var emote in sevenTvChannelEmotes)
            {
                emoteNames.Add(emote.Name);
            }
        }

        return emoteNames;
    }

    #region Bot_On

    private static void Client_OnConnected(object sender, EventArgs e)
    {
        ConsoleOut("[TWITCH] CONNECTED", ConsoleColor.Red, true);
    }

    private async ValueTask Client_OnJoinedChannel(JoinedChannelArgs e)
    {
        if (e.Username == AppSettings.Twitch.Username)
        {
            ConsoleOut($"[TWITCH] JOINED: <#{e.Channel}>", ConsoleColor.Red);
            return;
        }

        if (e.Channel != AppSettings.OfflineChatChannel)
        {
            return;
        }

        var user = await TwitchApi.GetUserAsync(e.Username);
        if (user is null)
        {
            return;
        }

        if (!AppSettings.UserLists.SecretUsers.Contains(user.Id))
        {
            await SendAsync(AppSettings.OfflineChatChannel, $"{e.Username} joined the chat Stare");
        }
    }

    private async ValueTask Client_OnMessageReceived(ChatMessage message)
    {
        ConsoleOut($"[TWITCH] <#{message.Channel}> {message.Username}: {message.Message}");
        await _messageHandler.Handle(message);
    }

    private static void Client_OnDisconnect(object sender, EventArgs e)
    {
        ConsoleOut("[TWITCH] DISCONNECTED", ConsoleColor.Red, true);
    }

    #endregion Bot_On

    #region Timer

    private void InitializeTimers()
    {
        _timerCollection.Add(OnTimer1000, TimeSpan.FromSeconds(1).TotalMilliseconds, startDirectly: false);
    }

    private async ValueTask OnTimer1000(object? sender, ElapsedEventArgs e)
    {
        Reminder[] reminders = Reminders.GetExpiredReminders();
        for (int i = 0; i < reminders.Length; i++)
        {
            await this.SendTimedReminder(reminders[i]);
        }
    }

    #endregion Timer
}
