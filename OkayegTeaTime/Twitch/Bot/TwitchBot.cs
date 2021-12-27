using System.Diagnostics;
using System.Timers;
using HLE.Numbers;
using HLE.Strings;
using HLE.Time;
using OkayegTeaTime.Database;
using OkayegTeaTime.Logging;
using OkayegTeaTime.Twitch.Api;
using OkayegTeaTime.Twitch.Bot.EmoteManagementNotifications;
using OkayegTeaTime.Twitch.Commands;
using OkayegTeaTime.Twitch.Messages;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Twitch.Whisper;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Enums;
using TwitchLib.Communication.Events;
using TwitchLib.Communication.Models;
using static HLE.Time.TimeHelper;

namespace OkayegTeaTime.Twitch.Bot;

public class TwitchBot
{
    public TwitchClient TwitchClient { get; private set; }

    public ConnectionCredentials ConnectionCredentials { get; private set; }

    public ClientOptions ClientOptions { get; private set; }

    public WebSocketClient WebSocketClient { get; private set; }

    public TcpClient TcpClient { get; private set; }

    public MessageHandler MessageHandler { get; private set; }

    public WhisperHandler WhisperHandler { get; private set; }

    public LastMessagesDictionary LastMessagesDictionary { get; private set; } = new();

    public Restarter Restarter { get; private set; } = new(new() { new(4, 0), new(4, 10), new(4, 20), new(4, 30), new(4, 40), new(4, 50), new(5, 0) });

    public EmoteManagementNotificator EmoteManagementNotificator { get; private set; }

    public DottedNumber CommandCount { get; set; } = 1;

    public string SystemInfo => GetSystemInfo();

    public string MemoryUsage => GetMemoryUsage();

    public string Runtime => ConvertUnixTimeToTimeStamp(_runtime);

    public static List<Timer> Timers { get; } = new();

    public static List<Cooldown> Cooldowns { get; } = new();

    public static List<AfkCooldown> AfkCooldowns { get; } = new();

    private readonly long _runtime = Now();

    public TwitchBot()
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

#if DEBUG
        TwitchClient.Initialize(ConnectionCredentials, AppSettings.DebugChannel);
#else
        TwitchClient.Initialize(ConnectionCredentials, DatabaseController.GetChannels());
#endif

        TwitchClient.OnLog += Client_OnLog;
        TwitchClient.OnConnected += Client_OnConnected;
        TwitchClient.OnJoinedChannel += Client_OnJoinedChannel;
        TwitchClient.OnMessageReceived += Client_OnMessageReceived;
        TwitchClient.OnMessageSent += Client_OnMessageSent;
        TwitchClient.OnWhisperReceived += Client_OnWhisperReceived;
        TwitchClient.OnConnectionError += Client_OnConnectionError;
        TwitchClient.OnError += Client_OnError;
        TwitchClient.OnDisconnected += Client_OnDisconnect;
        TwitchClient.OnReconnected += Client_OnReconnected;
        TwitchClient.OnUserJoined += Client_OnUserJoinedChannel;
    }

    public void Connect()
    {
        TwitchClient.Connect();
        Initlialize();
    }

    public void Send(Channel channel, string message)
    {
        if (!MessageHelper.IsMessageTooLong(message, channel))
        {
            message = message == LastMessagesDictionary[channel.Name] ? $"{message} {AppSettings.ChatterinoChar}" : message;
            message = $"{channel.Emote} {message}";
            TwitchClient.SendMessage(channel.Name, message);
            LastMessagesDictionary[channel.Name] = message;
        }
        else
        {
            new DividedMessage(this, channel, channel.Emote, message).StartSending();
        }
    }

    public void Send(string channel, string message)
    {
        Send(new Channel(channel), message);
    }

    public string JoinChannel(string channel)
    {
        if (TwitchApi.DoesUserExist(channel))
        {
            DatabaseController.AddChannel(channel);
            LastMessagesDictionary.Add(channel);
            try
            {
                TwitchClient.JoinChannel(channel);
                Send(channel, "I'm online");
                return $"successfully joined #{channel}";
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return $"unable to join #{channel}";
            }
        }
        else
        {
            return $"channel #{channel} does not exist";
        }
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
        //ConsoleOut($"[TWITCH] {e.Data}");
    }

    private void Client_OnConnected(object sender, OnConnectedArgs e)
    {
        ConsoleOut("[TWITCH] CONNECTED", true, ConsoleColor.Red);
    }

    private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
    {
        ConsoleOut($"[TWITCH] JOINED: <#{e.Channel}>", fontColor: ConsoleColor.Red);
    }

    private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
    {
        ConsoleOut($"[TWITCH] <#{e.ChatMessage.Channel}> {e.ChatMessage.Username}: {e.ChatMessage.Message.RemoveChatterinoChar().TrimAll()}");
        MessageHandler.CheckForPajaAlert(e.ChatMessage);
        MessageHandler.Handle(new TwitchChatMessage(e.ChatMessage));
    }

    private void Client_OnMessageSent(object sender, OnMessageSentArgs e)
    {
        ConsoleOut($"[TWITCH] <#{e.SentMessage.Channel}> {AppSettings.Twitch.Username}: {e.SentMessage.Message}", fontColor: ConsoleColor.Green);
    }

    private void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
    {
        ConsoleOut($"[TWITCH] <WHISPER> {e.WhisperMessage.Username}: {e.WhisperMessage.Message}");
        WhisperHandler.Handle(new TwitchWhisperMessage(e.WhisperMessage));
    }

    private void Client_OnConnectionError(object sender, OnConnectionErrorArgs e)
    {
        ConsoleOut($"[TWITCH] <CONNECTION-ERROR> {e.Error.Message}", true, ConsoleColor.Red);
        Restart();
    }

    private void Client_OnError(object sender, OnErrorEventArgs e)
    {
        ConsoleOut($"[TWITCH] <ERROR> {e.Exception.Message}", true, ConsoleColor.Red);
        Restart();
    }

    private void Client_OnDisconnect(object sender, OnDisconnectedEventArgs e)
    {
        ConsoleOut($"[TWITCH] DISCONNECTED", true, ConsoleColor.Red);
        Restart();
    }

    private void Client_OnReconnected(object sender, OnReconnectedEventArgs e)
    {
        ConsoleOut($"[TWITCH] RECONNECTED", true, ConsoleColor.Red);
    }

    private void Client_OnUserJoinedChannel(object sender, OnUserJoinedArgs e)
    {
        if (e.Channel == AppSettings.SecretOfflineChatChannel && !AppSettings.UserLists.SecretUsers.Contains(e.Username))
        {
            Send(AppSettings.SecretOfflineChatChannel, $"{e.Username} joined the chat Stare");
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
        Bot.Timers.GetTimer(new Second().Milliseconds).Elapsed += OnTimer1000;
        Bot.Timers.GetTimer(new Second(30).Milliseconds).Elapsed += OnTimer30000;
        Bot.Timers.GetTimer(new Minute().Milliseconds).Elapsed += OnTimer60000;
        Bot.Timers.GetTimer(new Day(10).Milliseconds).Elapsed += OnTimer10Days;
    }

    private void OnTimer1000(object sender, ElapsedEventArgs e)
    {
        TimerFunctions.CheckForTimedReminders(this);
    }

    private void OnTimer30000(object sender, ElapsedEventArgs e)
    {
        TimerFunctions.SetConsoleTitle(this);
    }

    private void OnTimer60000(object sender, ElapsedEventArgs e)
    {
        TimerFunctions.ReloadJsonFiles();
    }

    private void OnTimer10Days(object sender, ElapsedEventArgs e)
    {
        TimerFunctions.TwitchApiRefreshAccessToken();
    }

    #endregion Timer

    private void Initlialize()
    {
        MessageHandler = new(this);
        WhisperHandler = new(this);
        EmoteManagementNotificator = new(this);
        Restarter.InitializeResartTimer();
        InitializeTimers();
    }
}
