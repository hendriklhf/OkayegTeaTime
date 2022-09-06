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
using OkayegTeaTime.Twitch.Models;
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

public class TwitchBot
{
    public UserCache Users { get; } = new();

    public ReminderCache Reminders { get; } = new();

    public ChannelCache Channels { get; } = new();

    public SpotifyUserCache SpotifyUsers { get; } = new();

    public EmoteController EmoteController { get; }

    public CommandController CommandController { get; }

    public WeatherController WeatherController { get; } = new();

    public TwitchApi TwitchApi { get; } = new();

    public uint CommandCount { get; set; } = 1;

    public DateTime StartTime { get; } = DateTime.Now;

    private readonly TwitchClient _twitchClient;
    private readonly MessageHandler _messageHandler;
    private readonly TimerController _timerController = new();
    private readonly LastMessageController _lastMessageController;

    private readonly Restarter _restarter = new(new[]
    {
        (4, 0),
        (4, 10),
        (4, 20),
        (4, 30),
        (4, 40),
        (4, 50),
        (5, 0)
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

        CommandController = new();
        EmoteController = new(Channels);
        _messageHandler = new(this);
        _lastMessageController = new(Channels);
        _restarter.Initialize();
    }

    public void Connect()
    {
        _twitchClient.Connect();
        InitializeTimers();
    }

    public void Send(string channel, string message)
    {
        string emote = Channels[channel]?.Emote ?? AppSettings.DefaultEmote;
        message = message == _lastMessageController[channel]
            ? string.Join(' ', emote, message, AppSettings.ChatterinoChar)
            : string.Join(' ', emote, message);
        if (message.Length <= AppSettings.MaxMessageLength)
        {
            _twitchClient.SendMessage(channel, message);
        }
        else
        {
            DividedMessage dividedMessage = new(this, channel, message);
            dividedMessage.StartSending();
        }

        _lastMessageController[channel] = message;
    }

    public void SendText(string channel, string message)
    {
        if (message == _lastMessageController[channel])
        {
            message = $"{message} {AppSettings.ChatterinoChar}";
        }

        _twitchClient.SendMessage(channel, message);
        _lastMessageController[channel] = message;
    }

    public string JoinChannel(string channel)
    {
        User? user = TwitchApi.GetUser(channel);
        if (user is null)
        {
            return $"channel #{channel} does not exist";
        }

        Channel? chnl = Channels[channel];
        if (chnl is not null)
        {
            return $"the bot is already connected to #{channel}";
        }

        Channels.Add(long.Parse(user.Id), channel);
        try
        {
            _twitchClient.JoinChannel(channel);
            Send(channel, $"{Emoji.Wave} hello");
            return $"successfully joined #{channel}";
        }
        catch (Exception ex)
        {
            DbController.LogException(ex);
            return $"unable to join #{channel}";
        }
    }

    public string LeaveChannel(string channel)
    {
        try
        {
            Send(channel, $"{Emoji.Wave} bye");
            Channels.Remove(channel);
            _twitchClient.LeaveChannel(channel);
            return $"successfully left #{channel}";
        }
        catch (Exception ex)
        {
            DbController.LogException(ex);
            return $"unable to leave #{channel}";
        }
    }

    public void InvalidateCaches()
    {
        Reminders.Invalidate();
        SpotifyUsers.Invalidate();
        Users.Invalidate();
        Channels.Invalidate();
    }

    public void ReceiveMessage(TwitchChatMessage chatMessage)
    {
        _messageHandler.Handle(chatMessage);
    }

    #region Bot_On

    private void Client_OnLog(object sender, OnLogArgs e)
    {
        ConsoleOut($"[TWITCH] {e.Data}", ConsoleColor.Blue);
    }

    private void Client_OnConnected(object sender, OnConnectedArgs e)
    {
        ConsoleOut("[TWITCH] CONNECTED", ConsoleColor.Red, true);
    }

    private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
    {
        ConsoleOut($"[TWITCH] JOINED: <#{e.Channel}>", ConsoleColor.Red);
    }

    private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
    {
        ConsoleOut($"[TWITCH] <#{e.ChatMessage.Channel}> {e.ChatMessage.Username}: {e.ChatMessage.Message.TrimAll()}");
        _messageHandler.Handle(new TwitchChatMessage(e.ChatMessage));
    }

    private void Client_OnMessageSent(object sender, OnMessageSentArgs e)
    {
        ConsoleOut($"[TWITCH] <#{e.SentMessage.Channel}> {AppSettings.Twitch.Username}: {e.SentMessage.Message}", ConsoleColor.Green);
    }

    private void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
    {
        ConsoleOut($"[TWITCH] <WHISPER> {e.WhisperMessage.Username}: {e.WhisperMessage.Message}");
    }

    private void Client_OnConnectionError(object sender, OnConnectionErrorArgs e)
    {
        ConsoleOut($"[TWITCH] <CONNECTION-ERROR> {e.Error.Message}", ConsoleColor.Red, true);
        Restart();
    }

    private void Client_OnError(object sender, OnErrorEventArgs e)
    {
        ConsoleOut($"[TWITCH] <ERROR> {e.Exception.Message}", ConsoleColor.Red, true);
        Restart();
    }

    private void Client_OnDisconnect(object sender, OnDisconnectedEventArgs e)
    {
        ConsoleOut("[TWITCH] DISCONNECTED", ConsoleColor.Red, true);
        Restart();
    }

    private void Client_OnReconnected(object sender, OnReconnectedEventArgs e)
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
        _timerController.Add(OnTimer1000, 1000);
        _timerController.Add(OnTimer10Days, TimeSpan.FromDays(10).TotalMilliseconds);
    }

    private void OnTimer1000(object? sender, ElapsedEventArgs e)
    {
        IEnumerable<Reminder> reminders = Reminders.GetExpiredReminders();
        reminders.ForEach(this.SendTimedReminder);
    }

    private void OnTimer10Days(object? sender, ElapsedEventArgs e)
    {
        TwitchApi.RefreshAccessToken();
    }

    #endregion Timer
}
