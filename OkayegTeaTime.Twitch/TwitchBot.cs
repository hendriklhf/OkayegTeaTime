using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using HLE.Emojis;
using HLE.Twitch;
using HLE.Twitch.Models;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Cache;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Controller;
using OkayegTeaTime.Twitch.Handlers;
using OkayegTeaTime.Twitch.Messages;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;
using static OkayegTeaTime.Utils.ProcessUtils;
using User = TwitchLib.Api.Helix.Models.Users.GetUsers.User;

namespace OkayegTeaTime.Twitch;

public sealed class TwitchBot
{
    public UserCache Users { get; } = new();

    public ReminderCache Reminders { get; } = new();

    public ChannelCache Channels { get; } = new();

    public SpotifyUserCache SpotifyUsers { get; } = new();

    public EmoteController EmoteController { get; }

    public CommandController CommandController { get; }

    public WeatherController WeatherController { get; } = new();

    public TwitchApi TwitchApi { get; } = new();

    public RegexCreator RegexCreator { get; set; } = new();

    public LastMessageController LastMessages { get; }

    public Dictionary<long, HangmanGame> HangmanGames { get; } = new();

    public uint CommandCount { get; set; }

    public DateTime StartTime { get; } = DateTime.UtcNow;

    public TimeSpan Uptime => DateTime.UtcNow - StartTime;

    private readonly TwitchClient _twitchClient;
    private readonly MessageHandler _messageHandler;
    private readonly TimerCollection _timerCollection = new();

    public TwitchBot(IEnumerable<string>? channels = null)
    {
        _twitchClient = new(AppSettings.Twitch.Username, AppSettings.Twitch.OAuthToken, new()
        {
            ClientType = ClientType.WebSocket,
            UseSSL = true
        });

        channels ??= Channels.Select(c => c.Name);
        _twitchClient.JoinChannels(channels.ToArray());

        //_twitchClient.OnLog += Client_OnLog!;
        _twitchClient.OnConnected += Client_OnConnected!;
        _twitchClient.OnJoinedChannel += Client_OnJoinedChannel!;
        _twitchClient.OnChatMessageReceived += Client_OnMessageReceived!;
        _twitchClient.OnDisconnected += Client_OnDisconnect!;

        CommandController = new(this);
        EmoteController = new(Channels);
        LastMessages = new(Channels);
        _messageHandler = new(this);
        InitializeTimers();
    }

    public void Connect()
    {
        _twitchClient.ConnectAsync().Wait();
        _timerCollection.StartAll();
    }

    public void Disconnect()
    {
        _timerCollection.StopAll();
        _twitchClient.DisconnectAsync().Wait();
    }

    public void Send(string channel, string message, bool addEmote = true, bool checkLength = true, bool checkDuplicate = true)
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

        if (checkLength && message.Length >= AppSettings.MaxMessageLength)
        {
            DividedMessage dividedMessage = new(this, channel, message);
            dividedMessage.Send();
        }
        else
        {
            _twitchClient.Send(_twitchClient.Channels[channel]!.Id, message);
        }

        LastMessages[channel] = message;
    }

    public bool JoinChannel(string channel)
    {
        try
        {
            _twitchClient.JoinChannel(channel);
            Send(channel, $"{Emoji.Wave} hello");
            long channelId = TwitchApi.GetUserId(channel);
            Channels.Add(channelId, channel);
            return true;
        }
        catch (Exception ex)
        {
            DbController.LogException(ex);
            return false;
        }
    }

    public bool LeaveChannel(string channel)
    {
        try
        {
            Send(channel, $"{Emoji.Wave} bye");
            _twitchClient.LeaveChannel(channel);
            Channels.Remove(channel);
            return true;
        }
        catch (Exception ex)
        {
            DbController.LogException(ex);
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

    /*private void Client_OnLog(object sender, OnLogArgs e)
    {
        ConsoleOut($"[TWITCH] {e.Data}", ConsoleColor.Blue);
    }*/

    private static void Client_OnConnected(object sender, EventArgs e)
    {
        ConsoleOut("[TWITCH] CONNECTED", ConsoleColor.Red, true);
    }

    private void Client_OnJoinedChannel(object sender, JoinedChannelArgs e)
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

        User? user = TwitchApi.GetUser(e.Username);
        if (user is null)
        {
            return;
        }

        if (!AppSettings.UserLists.SecretUsers.Contains(long.Parse(user.Id)))
        {
            Send(AppSettings.OfflineChatChannel, $"{e.Username} joined the chat Stare");
        }
    }

    private void Client_OnMessageReceived(object sender, ChatMessage e)
    {
        ConsoleOut($"[TWITCH] <#{e.Channel}> {e.Username}: {e.Message}");
        _messageHandler.Handle(e);
    }

    private static void Client_OnDisconnect(object sender, EventArgs e)
    {
        ConsoleOut("[TWITCH] DISCONNECTED", ConsoleColor.Red, true);
        Restart();
    }

    #endregion Bot_On

    #region Timer

    private void InitializeTimers()
    {
        _timerCollection.Add(OnTimer1000, TimeSpan.FromSeconds(1).TotalMilliseconds, startDirectly: false);
    }

    private void OnTimer1000(object? sender, ElapsedEventArgs e)
    {
        Span<Reminder> reminders = Reminders.GetExpiredReminders();
        for (int i = 0; i < reminders.Length; i++)
        {
            this.SendTimedReminder(reminders[i]);
        }
    }

    #endregion Timer
}
