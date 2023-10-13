using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using HLE.Emojis;
using HLE.Twitch;
using HLE.Twitch.Models;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Cache;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Bttv;
using OkayegTeaTime.Twitch.Controller;
using OkayegTeaTime.Twitch.Ffz;
using OkayegTeaTime.Twitch.Handlers;
using OkayegTeaTime.Twitch.Helix;
using OkayegTeaTime.Twitch.Messages;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Twitch.Services;
using OkayegTeaTime.Twitch.SevenTv;
using static OkayegTeaTime.Utils.ProcessUtils;

namespace OkayegTeaTime.Twitch;

public sealed class TwitchBot : IDisposable, IEquatable<TwitchBot>
{
    public UserCache Users { get; } = new();

    public ReminderCache Reminders { get; } = new();

    public ChannelCache Channels { get; } = new();

    public SpotifyUserCache SpotifyUsers { get; } = new();

    public CommandController CommandController { get; } = new();

    public WeatherService WeatherService { get; } = new();

    public CooldownController CooldownController { get; }

    public TwitchApi TwitchApi { get; } = new(AppSettings.Twitch.ApiClientId, AppSettings.Twitch.ApiClientSecret, new());

    public FfzApi FfzApi { get; } = new(new());

    public BttvApi BttvApi { get; } = new(new());

    public SevenTvApi SevenTvApi { get; } = new(new());

    public MessageRegexCreator MessageRegexCreator { get; set; } = new();

    public LastMessageController LastMessages { get; }

    public Dictionary<long, HangmanGame> HangmanGames { get; } = new();

    public EmoteService EmoteService { get; }

    public DotNetFiddleService DotNetFiddleService { get; } = new();

    public MathService MathService { get; } = new();

    public AfkMessageBuilder AfkMessageBuilder { get; }

    public uint CommandCount { get; set; }

    private readonly TwitchClient _twitchClient;
    private readonly MessageHandler _messageHandler;
    private readonly PeriodicActionsController _periodicActionsController;

    public TwitchBot(ReadOnlyMemory<string> channels)
    {
        OAuthToken token = new(AppSettings.Twitch.OAuthToken);
        _twitchClient = new(AppSettings.Twitch.Username, token, new()
        {
            UseSSL = true,
            ParsingMode = ParsingMode.MemoryEfficient
        });

        if (channels.Length == 0)
        {
            channels = Channels.Select(static c => c.Name).ToArray();
        }

        _twitchClient.JoinChannelsAsync(channels).AsTask().Wait(); // TODO: 💢

        _twitchClient.OnConnected += Client_OnConnected!;
        _twitchClient.OnJoinedChannel += async (_, e) => await Client_OnJoinedChannel(e);
        _twitchClient.OnChatMessageReceived += async (_, msg) => await Client_OnMessageReceived(msg);
        _twitchClient.OnDisconnected += Client_OnDisconnect!;

        CooldownController = new(CommandController);
        LastMessages = new(Channels);
        _messageHandler = new(this);
        EmoteService = new(this);
        _periodicActionsController = new(GetPeriodicActions());
        AfkMessageBuilder = new(CommandController.AfkCommands.AsSpan());
    }

    public async ValueTask ConnectAsync()
    {
        await _twitchClient.ConnectAsync();
        _periodicActionsController.StartAll();
    }

    public async ValueTask DisconnectAsync()
    {
        _periodicActionsController.StopAll();
        await _twitchClient.DisconnectAsync();
    }

    public async ValueTask SendAsync(long channelId, ReadOnlyMemory<char> message)
    {
        await _twitchClient.SendAsync(channelId, message);
    }

    public async ValueTask SendAsync(string channel, ReadOnlyMemory<char> message)
    {
        await _twitchClient.SendAsync(channel, message);
    }

    public async ValueTask SendAsync(ReadOnlyMemory<char> channel, ReadOnlyMemory<char> message)
    {
        await _twitchClient.SendAsync(channel, message);
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

    public async ValueTask<bool> JoinChannelAsync(string channel)
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

    private static void Client_OnConnected(object sender, EventArgs e)
    {
        ConsoleOut("[TWITCH] CONNECTED", ConsoleColor.Red, true);
    }

    private async ValueTask Client_OnJoinedChannel(JoinChannelMessage e)
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

    private async ValueTask Client_OnMessageReceived(IChatMessage message)
    {
        ConsoleOut($"[TWITCH] <#{message.Channel}> {message.Username}: {message.Message}");
        await _messageHandler.Handle(message);

        if (message is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    private static void Client_OnDisconnect(object sender, EventArgs e)
    {
        ConsoleOut("[TWITCH] DISCONNECTED", ConsoleColor.Red, true);
    }

    #endregion Bot_On

    #region Timer

    private PeriodicAction[] GetPeriodicActions()
    {
        return new PeriodicAction[]
        {
            new(CheckForExpiredReminders, TimeSpan.FromSeconds(1))
        };
    }

    private async ValueTask CheckForExpiredReminders()
    {
        Reminder[] reminders = Reminders.GetExpiredReminders();
        for (int i = 0; i < reminders.Length; i++)
        {
            await this.SendTimedReminder(reminders[i]);
        }
    }

    #endregion Timer

    public bool Equals(TwitchBot? other)
    {
        return ReferenceEquals(this, other);
    }

    public override bool Equals(object? obj)
    {
        return obj is TwitchBot other && Equals(other);
    }

    public override int GetHashCode()
    {
        return RuntimeHelpers.GetHashCode(this);
    }

    public static bool operator ==(TwitchBot? left, TwitchBot? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(TwitchBot? left, TwitchBot? right)
    {
        return !(left == right);
    }

    public void Dispose()
    {
        _twitchClient.Dispose();
        _periodicActionsController.Dispose();
        TwitchApi.Dispose();
        MessageRegexCreator.Dispose();
    }
}
