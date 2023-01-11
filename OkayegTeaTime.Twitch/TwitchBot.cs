using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using HLE;
using HLE.Collections;
using HLE.Emojis;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Cache;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Files;
using OkayegTeaTime.Twitch.Controller;
using OkayegTeaTime.Twitch.Handlers;
using OkayegTeaTime.Twitch.Messages;
using OkayegTeaTime.Utils;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Enums;
using TwitchLib.Communication.Events;
using TwitchLib.Communication.Models;
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

    public uint CommandCount { get; set; }

    public DateTime StartTime { get; } = DateTime.UtcNow;

    public long Latency { get; private set; }

    private readonly TwitchClient _twitchClient;
    private readonly MessageHandler _messageHandler;
    private readonly TimerCollection _timerCollection = new();

    private readonly Restarter _restarter = new(new TimeOnly[]
    {
        new(4, 0),
        new(4, 10),
        new(4, 20),
        new(4, 30),
        new(4, 40),
        new(4, 50),
        new(5, 0)
    });

    public TwitchBot(IEnumerable<string>? channels = null, IEnumerable<string>? excludedChannels = null)
    {
        ConnectionCredentials connectionCredentials = new(AppSettings.Twitch.Username, AppSettings.Twitch.OAuthToken);
        ClientOptions clientOptions = new()
        {
            ClientType = ClientType.Chat,
            ReconnectionPolicy = new(10000, 30000, 1000),
            UseSsl = true
        };
        TcpClient tcpClient = new(clientOptions);
        _twitchClient = new(tcpClient, ClientProtocol.TCP)
        {
            AutoReListenOnException = true
        };

        channels ??= Channels.Select(c => c.Name);
        if (excludedChannels is not null)
        {
            channels = channels.Except(excludedChannels);
        }

        _twitchClient.Initialize(connectionCredentials, channels.ToList());

        //_twitchClient.OnLog += Client_OnLog!;
        _twitchClient.OnConnected += Client_OnConnected!;
        _twitchClient.OnJoinedChannel += Client_OnJoinedChannel!;
        _twitchClient.OnMessageReceived += Client_OnMessageReceived!;
        _twitchClient.OnMessageSent += Client_OnMessageSent!;
        _twitchClient.OnWhisperReceived += Client_OnWhisperReceived!;
        _twitchClient.OnConnectionError += Client_OnConnectionError!;
        _twitchClient.OnError += Client_OnError!;
        _twitchClient.OnDisconnected += Client_OnDisconnect!;
        _twitchClient.OnReconnected += Client_OnReconnected!;
        _twitchClient.OnUserJoined += Client_OnUserJoinedChannel!;

        CommandController = new(this);
        EmoteController = new(Channels);
        LastMessages = new(Channels);
        _messageHandler = new(this);
        _restarter.Start();
        InitializeTimers();
    }

    public void Connect()
    {
        _twitchClient.Connect();
        _timerCollection.StartAll();
    }

    public void Disconnect()
    {
        _timerCollection.StopAll();
        _twitchClient.Disconnect();
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
            _twitchClient.SendMessage(channel, message);
        }

        LastMessages[channel] = message;
    }

    public void SendText(string channel, string message)
    {
        if (message == LastMessages[channel])
        {
            message = $"{message} {AppSettings.ChatterinoChar}";
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

    private static void Client_OnConnected(object sender, OnConnectedArgs e)
    {
        ConsoleOut("[TWITCH] CONNECTED", ConsoleColor.Red, true);
    }

    private static void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
    {
        ConsoleOut($"[TWITCH] JOINED: <#{e.Channel}>", ConsoleColor.Red);
    }

    private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
    {
        Latency = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - long.Parse(e.ChatMessage.TmiSentTs);
        ConsoleOut($"[TWITCH] <#{e.ChatMessage.Channel}> {e.ChatMessage.Username}: {e.ChatMessage.Message.TrimAll()}");
        _messageHandler.Handle(new(e.ChatMessage));
    }

    private static void Client_OnMessageSent(object sender, OnMessageSentArgs e)
    {
        ConsoleOut($"[TWITCH] <#{e.SentMessage.Channel}> {AppSettings.Twitch.Username}: {e.SentMessage.Message}", ConsoleColor.Green);
    }

    private static void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
    {
        ConsoleOut($"[TWITCH] <WHISPER> {e.WhisperMessage.Username}: {e.WhisperMessage.Message}");
    }

    private static void Client_OnConnectionError(object sender, OnConnectionErrorArgs e)
    {
        ConsoleOut($"[TWITCH] <CONNECTION-ERROR> {e.Error.Message}", ConsoleColor.Red, true);
        Restart();
    }

    private static void Client_OnError(object sender, OnErrorEventArgs e)
    {
        ConsoleOut($"[TWITCH] <ERROR> {e.Exception.Message}", ConsoleColor.Red, true);
        Restart();
    }

    private static void Client_OnDisconnect(object sender, OnDisconnectedEventArgs e)
    {
        ConsoleOut("[TWITCH] DISCONNECTED", ConsoleColor.Red, true);
        Restart();
    }

    private static void Client_OnReconnected(object sender, OnReconnectedEventArgs e)
    {
        ConsoleOut("[TWITCH] RECONNECTED", ConsoleColor.Red, true);
    }

    private void Client_OnUserJoinedChannel(object sender, OnUserJoinedArgs e)
    {
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

    #endregion Bot_On

    #region Timer

    private void InitializeTimers()
    {
        _timerCollection.Add(OnTimer1000, TimeSpan.FromSeconds(1).TotalMilliseconds, startDirectly: false);
    }

    private void OnTimer1000(object? sender, ElapsedEventArgs e)
    {
        Reminder[] reminders = Reminders.GetExpiredReminders();
        reminders.ForEach(this.SendTimedReminder);
    }

    #endregion Timer
}
