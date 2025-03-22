using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using HLE.Text;
using HLE.Twitch.Tmi;
using HLE.Twitch.Tmi.Models;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Cache;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Configuration;
using OkayegTeaTime.Twitch.Bttv;
using OkayegTeaTime.Twitch.Controller;
using OkayegTeaTime.Twitch.Ffz;
using OkayegTeaTime.Twitch.Handlers;
using OkayegTeaTime.Twitch.Helix;
using OkayegTeaTime.Twitch.Messages;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Twitch.Services;
using OkayegTeaTime.Twitch.SevenTv;
using OkayegTeaTime.Utils;
using Channel = HLE.Twitch.Tmi.Models.Channel;
using User = OkayegTeaTime.Twitch.Helix.Models.User;

namespace OkayegTeaTime.Twitch;

public sealed partial class TwitchBot : IDisposable, IEquatable<TwitchBot>
{
    public UserCache Users { get; } = [];

    public ReminderCache Reminders { get; } = [];

    public ChannelCache Channels { get; } = [];

    public SpotifyUserCache SpotifyUsers { get; } = [];

    public WeatherService WeatherService { get; } = new();

    public CooldownController CooldownController { get; } = new();

    public TwitchApi TwitchApi { get; } = new(GlobalSettings.Settings.Twitch.ApiClientId, GlobalSettings.Settings.Twitch.ApiClientSecret, new());

    public FfzApi FfzApi { get; } = new(new());

    public BttvApi BttvApi { get; } = new(new());

    public SevenTvApi SevenTvApi { get; } = new(new());

    public MessageRegexCreator MessageRegexCreator { get; set; } = new();

    public LastMessageController LastMessages { get; } = new();

    public ConcurrentDictionary<long, HangmanGame> HangmanGames { get; } = [];

    public EmoteService EmoteService { get; }

    public MathService MathService { get; } = new();

    public AfkMessageBuilder AfkMessageBuilder { get; }

    public uint CommandCount => _commandCount;

    private readonly TwitchClient _twitchClient;
    private readonly MessageHandler _messageHandler;
    private readonly PeriodicActionsController _periodicActionsController;
    private uint _commandCount;

    public TwitchBot(ReadOnlyMemory<string> channels)
    {
        OAuthToken token = new(GlobalSettings.Settings.Twitch.OAuthToken);
        TwitchClient twitchClient = new(GlobalSettings.Settings.Twitch.Username, token, ClientOptions.Default);

        if (channels.Length == 0)
        {
            channels = Channels.Select(static c => c.Name).ToArray();
        }

#pragma warning disable S4462, VSTHRD002
        twitchClient.JoinChannelsAsync(channels).AsTask().Wait(); // TODO: 💢
#pragma warning restore VSTHRD002, S4462

        twitchClient.OnConnected += OnClientConnectedAsync;
        twitchClient.OnJoinedChannel += OnClientJoinedChannelAsync;
        twitchClient.OnChatMessageReceived += OnClientMessageReceivedAsync;
        twitchClient.OnDisconnected += OnClientDisconnectAsync;

        _twitchClient = twitchClient;
        _messageHandler = new(this);
        EmoteService = new(this);
        _periodicActionsController = new(GetPeriodicActions());
        AfkMessageBuilder = new(CommandController.AfkCommands.AsSpan());
    }

    public void IncrementCommandCount() => Interlocked.Increment(ref _commandCount);

    [Pure]
    public bool IsConnectedTo(ReadOnlySpan<char> channel) => _twitchClient.Channels.TryGet(channel, out _);

    public async ValueTask ConnectAsync()
    {
        await _twitchClient.ConnectAsync();
        _periodicActionsController.StartAll();
    }

    public ValueTask SendAsync(long channelId, ReadOnlyMemory<char> message) => _twitchClient.SendAsync(channelId, message);

    public ValueTask SendAsync(string channel, ReadOnlyMemory<char> message) => _twitchClient.SendAsync(channel, message);

    public ValueTask SendAsync(ReadOnlyMemory<char> channel, ReadOnlyMemory<char> message) => _twitchClient.SendAsync(channel, message);

    [SuppressMessage("Major Code Smell", "S4457:Parameter validation in \"async\"/\"await\" methods should be wrapped")]
    public async ValueTask SendAsync(string channel, string message, bool prependEmote = true, bool checkLength = true, bool checkDuplicate = true)
    {
        using PooledStringBuilder builder = new(message.Length << 1);

        if (prependEmote)
        {
            string emote = Channels[channel]?.Emote ?? GlobalSettings.DefaultEmote;
            builder.Append(emote);
            builder.Append(' ');
        }

        builder.Append(message);

        if (checkDuplicate && message == LastMessages[channel])
        {
            builder.Append(' ');
            builder.Append(GlobalSettings.ChatterinoChar);
        }

        message = checkLength && message.Length > GlobalSettings.MaxMessageLength
            ? new(builder.WrittenSpan[..GlobalSettings.MaxMessageLength])
            : builder.ToString();

        _twitchClient.Channels.TryGet(channel, out Channel? c);
        ArgumentNullException.ThrowIfNull(c);

        await _twitchClient.SendAsync(c.Id, message);
        LastMessages[channel] = message;
    }

    public async ValueTask<bool> JoinChannelAsync(string channel)
    {
        try
        {
            await _twitchClient.JoinChannelAsync(channel);
            await SendAsync(channel, $"{Emoji.Wave} hello");
            User? user = await TwitchApi.GetUserAsync(channel);
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

    public async ValueTask<bool> LeaveChannelAsync(string channel)
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

    #region Bot_On

    private static async Task OnClientConnectedAsync(TwitchClient _)
    {
        await using ConsoleWriter consoleWriter = new();
        consoleWriter.WriteConnected();
    }

    private async Task OnClientJoinedChannelAsync(TwitchClient _, JoinChannelMessage e)
    {
        if (e.Username == GlobalSettings.Settings.Twitch.Username)
        {
            await using ConsoleWriter consoleWriter = new();
            consoleWriter.WriteJoinedChannel(e.Channel);
            return;
        }

        if (e.Channel != GlobalSettings.Settings.OfflineChat!.Channel)
        {
            return;
        }

        User? user = await TwitchApi.GetUserAsync(e.Username);
        if (user is null)
        {
            return;
        }

        if (!GlobalSettings.Settings.OfflineChat.Users.Contains(user.Id))
        {
            await SendAsync(GlobalSettings.Settings.OfflineChat.Channel, $"{e.Username} joined the chat Stare");
        }
    }

    private async Task OnClientMessageReceivedAsync(TwitchClient _, ChatMessage message)
    {
        try
        {
            await _messageHandler.HandleAsync(message);
            await message.FormatAsync(Console.Out);
        }
        finally
        {
            message.Dispose();
        }
    }

    private static async Task OnClientDisconnectAsync(TwitchClient _)
    {
        await using ConsoleWriter consoleWriter = new();
        consoleWriter.WriteDisconnected();
    }

    #endregion Bot_On

    private PeriodicAction[] GetPeriodicActions() =>
    [
        new(CheckForExpiredRemindersAsync, TimeSpan.FromSeconds(1))
    ];

    private async ValueTask CheckForExpiredRemindersAsync()
    {
        Reminder[] reminders = Reminders.GetExpiredReminders();
        for (int i = 0; i < reminders.Length; i++)
        {
            await SendTimedReminderAsync(reminders[i]);
        }
    }

    public bool Equals(TwitchBot? other) => ReferenceEquals(this, other);

    public override bool Equals(object? obj) => obj is TwitchBot other && Equals(other);

    public override int GetHashCode() => RuntimeHelpers.GetHashCode(this);

    public static bool operator ==(TwitchBot? left, TwitchBot? right) => Equals(left, right);

    public static bool operator !=(TwitchBot? left, TwitchBot? right) => !(left == right);

    public void Dispose()
    {
        _twitchClient.Dispose();
        _periodicActionsController.Dispose();
        MathService.Dispose();
        TwitchApi.Dispose();
    }
}
