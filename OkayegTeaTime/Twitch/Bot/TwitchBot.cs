using System.Diagnostics;
using System.Timers;
using HLE.Emojis;
using HLE.Numbers;
using HLE.Strings;
using HLE.Time;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Logging;
using OkayegTeaTime.Twitch.Api;
using OkayegTeaTime.Twitch.Controller;
using OkayegTeaTime.Twitch.Handlers;
using OkayegTeaTime.Twitch.Messages;
using OkayegTeaTime.Twitch.Models;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Enums;
using TwitchLib.Communication.Events;
using TwitchLib.Communication.Models;
using static HLE.Time.TimeHelper;
using User = TwitchLib.Api.Helix.Models.Users.GetUsers.User;

namespace OkayegTeaTime.Twitch.Bot;

public class TwitchBot
{
    public TwitchClient TwitchClient { get; }

    public ConnectionCredentials ConnectionCredentials { get; }

    public ClientOptions ClientOptions { get; }

    public TcpClient TcpClient { get; }

    public List<string> Channels { get; }

    public MessageHandler? MessageHandler { get; private set; }

    public WhisperHandler? WhisperHandler { get; private set; }

    public LastMessagesDictionary LastMessagesDictionary { get; } = new();

    public EmoteController EmoteController { get; } = new();

    public CommandController CommandController { get; } = new();

    public Restarter Restarter { get; } = new(new()
    {
        new(4, 0),
        new(4, 10),
        new(4, 20),
        new(4, 30),
        new(4, 40),
        new(4, 50),
        new(5, 0)
    });

    public DottedNumber CommandCount { get; set; } = 1;

    public string SystemInfo => GetSystemInfo();

    public string Runtime => GetUnixDifference(_runtime).ToString();

    public static List<Timer> Timers { get; } = new();

    private readonly long _runtime = Now();

    public TwitchBot(IEnumerable<string>? channels = null)
    {
        TwitchApi.Initialize();

        ConnectionCredentials = new(AppSettings.Twitch.Username, AppSettings.Twitch.OAuthToken);
        ClientOptions = new()
        {
            ClientType = ClientType.Chat,
            ReconnectionPolicy = new(10000, 30000, 1000),
            UseSsl = true
        };
        TcpClient = new(ClientOptions);
        TwitchClient = new(TcpClient, ClientProtocol.TCP)
        {
            AutoReListenOnException = true
        };

        if (channels is not null)
        {
            Channels = channels.ToList();
        }
        else
        {
#if DEBUG
            Channels = new()
            {
                AppSettings.DebugChannel
            };
#else
            Channels = DbController.GetChannels().Select(c => c.Name).ToList();
#endif
        }

        TwitchClient.Initialize(ConnectionCredentials, Channels);

        TwitchClient.OnLog += Client_OnLog!;
        TwitchClient.OnConnected += Client_OnConnected!;
        TwitchClient.OnJoinedChannel += Client_OnJoinedChannel!;
        TwitchClient.OnMessageReceived += Client_OnMessageReceived!;
        TwitchClient.OnMessageSent += Client_OnMessageSent!;
        TwitchClient.OnWhisperReceived += Client_OnWhisperReceived!;
        TwitchClient.OnConnectionError += Client_OnConnectionError!;
        TwitchClient.OnError += Client_OnError!;
        TwitchClient.OnDisconnected += Client_OnDisconnect!;
        TwitchClient.OnReconnected += Client_OnReconnected!;
        TwitchClient.OnUserJoined += Client_OnUserJoinedChannel!;
    }

    public void Connect()
    {
        TwitchClient.Connect();
        Initlialize();
    }

    public void Send(string channel, string message)
    {
        string emote = DbControl.Channels[channel]?.Emote ?? AppSettings.DefaultEmote;
        if (!MessageHelper.IsMessageTooLong(message, channel))
        {
            message = message == LastMessagesDictionary[channel]
                ? string.Join(' ', emote, message, AppSettings.ChatterinoChar)
                : string.Join(' ', emote, message);
            TwitchClient.SendMessage(channel, message);
            LastMessagesDictionary[channel] = message;
        }
        else
        {
            new DividedMessage(this, channel, emote, message).StartSending();
        }
    }

    public string JoinChannel(string channel)
    {
        User? user = TwitchApi.GetUser(channel);
        if (user is null)
        {
            return $"channel #{channel} does not exist";
        }

        Channel? chnl = DbControl.Channels[channel];
        if (chnl is not null)
        {
            return $"the bot is already connected to #{channel}";
        }

        DbControl.Channels.Add(user.Id.ToInt(), channel);
        try
        {
            TwitchClient.JoinChannel(channel);
            Send(channel, $"{Emoji.Wave} hello");
            return $"successfully joined #{channel}";
        }
        catch (Exception ex)
        {
            Logger.Log(ex);
            return $"unable to join #{channel}";
        }
    }

    public string LeaveChannel(string channel)
    {
        try
        {
            Send(channel, $"{Emoji.Wave} bye");
            DbControl.Channels.Remove(channel);
            TwitchClient.LeaveChannel(channel);
            return $"successfully left #{channel}";
        }
        catch (Exception ex)
        {
            Logger.Log(ex);
            return $"unable to leave #{channel}";
        }
    }

    private void Initlialize()
    {
        MessageHandler = new(this);
        WhisperHandler = new();
        Restarter.Initialize();
        InitializeTimers();
    }

    #region SystemInfo

    private string GetSystemInfo()
    {
        return $"Uptime: {Runtime} || Memory usage: {GetMemoryUsage()} || Executed commands: {CommandCount}";
    }

    private string GetMemoryUsage()
    {
        return $"{Math.Truncate(Process.GetCurrentProcess().PrivateMemorySize64 / Math.Pow(10, 6) * 100) / 100}MB / 8000MB";
    }

    #endregion SystemInfo

    #region Bot_On

    private void Client_OnLog(object sender, OnLogArgs e)
    {
        //ConsoleOut($"[TWITCH] {e.Data}", ConsoleColor.Blue);
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
        MessageHandler?.Handle(new TwitchChatMessage(e.ChatMessage));
    }

    private void Client_OnMessageSent(object sender, OnMessageSentArgs e)
    {
        ConsoleOut($"[TWITCH] <#{e.SentMessage.Channel}> {AppSettings.Twitch.Username}: {e.SentMessage.Message}", ConsoleColor.Green);
    }

    private void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
    {
        ConsoleOut($"[TWITCH] <WHISPER> {e.WhisperMessage.Username}: {e.WhisperMessage.Message}");
        WhisperHandler?.Handle(new TwitchWhisperMessage(e.WhisperMessage));
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

        if (!AppSettings.UserLists.SecretUsers.Contains(user.Id.ToInt()))
        {
            Send(AppSettings.OfflineChatChannel, $"{e.Username} joined the chat Stare");
        }
    }

    #endregion Bot_On

    #region Timer

    private void InitializeTimers()
    {
        Bot.Timers.CreateTimers();
        AddTimerFunction();
        StartTimers();
    }

    private static void StartTimers()
    {
        Timers.ForEach(t => t.Start());
    }

    private void AddTimerFunction()
    {
        Bot.Timers.GetTimer(new Second().Milliseconds)!.Elapsed += OnTimer1000!;
        Bot.Timers.GetTimer(new Second(30).Milliseconds)!.Elapsed += OnTimer30000!;
        Bot.Timers.GetTimer(new Day(10).Milliseconds)!.Elapsed += OnTimer10Days!;
    }

    private void OnTimer1000(object sender, ElapsedEventArgs e)
    {
        TimerFunctions.CheckForTimedReminders(this);
    }

    private void OnTimer30000(object sender, ElapsedEventArgs e)
    {
        TimerFunctions.SetConsoleTitle(this);
    }

    private void OnTimer10Days(object sender, ElapsedEventArgs e)
    {
        TimerFunctions.TwitchApiRefreshAccessToken();
    }

    #endregion Timer
}
