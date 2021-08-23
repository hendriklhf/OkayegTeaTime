using HLE.Numbers;
using HLE.Time;
using OkayegTeaTimeCSharp.Database;
using OkayegTeaTimeCSharp.JsonData;
using OkayegTeaTimeCSharp.Properties;
using OkayegTeaTimeCSharp.Twitch.API;
using OkayegTeaTimeCSharp.Twitch.Messages;
using OkayegTeaTimeCSharp.Twitch.Whisper;
using OkayegTeaTimeCSharp.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Enums;
using TwitchLib.Communication.Events;
using TwitchLib.Communication.Models;
using static HLE.Time.TimeHelper;
using static OkayegTeaTimeCSharp.Program;

namespace OkayegTeaTimeCSharp.Twitch.Bot
{
    public class TwitchBot
    {
        public TwitchClient TwitchClient { get; private set; }

        public ConnectionCredentials ConnectionCredentials { get; private set; }

        public ClientOptions ClientOptions { get; private set; }

        public WebSocketClient WebSocketClient { get; private set; }

        public TcpClient TcpClient { get; private set; }

        public DottedNumber CommandCount { get; set; } = 1;

        public string Runtime => ConvertUnixTimeToTimeStamp(_runtime);

        public Restarter Restarter { get; } = new(new() { new(4, 0), new(4, 10), new(4, 20), new(4, 30), new(4, 40), new(4, 50), new(5, 0) });

        public static List<Timer> Timers { get; } = new();

        public static Dictionary<string, string> LastMessages { get; set; } = LastMessagesHelper.FillDictionary();

        public static Dictionary<string, string> Prefixes { get; set; } = PrefixHelper.FillDictionary();

        public static Dictionary<string, string> EmoteInFront { get; set; } = EmoteInFrontHelper.FillDictionary();

        public static List<Cooldown> Cooldowns { get; } = new();

        public static List<AfkCooldown> AfkCooldowns { get; } = new();

        private readonly long _runtime = Now();

        public TwitchBot()
        {
            ConnectionCredentials = new(Resources.Username, Resources.OAuthToken);
            ClientOptions = new()
            {
                ClientType = ClientType.Chat,
                ReconnectionPolicy = new(10000, 30000, 1000),
                UseSsl = true
            };
            //WebSocketClient = new(ClientOptions);
            TcpClient = new(ClientOptions);
            TwitchClient = new(TcpClient, ClientProtocol.TCP)
            {
                AutoReListenOnException = true
            };
#if DEBUG
            TwitchClient.Initialize(ConnectionCredentials, Resources.DebugChannel);
#else
            TwitchClient.Initialize(ConnectionCredentials, Config.Channels);
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

            TwitchClient.Connect();

            InitializeTimers();
            Restarter.InitializeResartTimer();
        }

        public void Send(string channel, string message)
        {
            if (!Config.NotAllowedChannels.Contains(channel.RemoveHashtag()))
            {
                string emoteInFront = EmoteInFrontHelper.GetEmote(channel);
                if ($"{emoteInFront} {message} {Resources.ChatterinoChar}".Length <= Config.MaxMessageLength)
                {
                    message = message == LastMessagesHelper.GetLastMessage(channel) ? $"{message} {Resources.ChatterinoChar}" : message;
                    TwitchClient.SendMessage(channel.RemoveHashtag(), $"{emoteInFront} {message}");
                    LastMessagesHelper.SetLastMessage(channel, message);
                }
                else
                {
                    new DividedMessage(this, channel, emoteInFront, message).StartSending();
                }
            }
        }

        public bool JoinChannel(string channel, out string responseMessage)
        {
            if (new TwitchAPI().GetChannelByName(channel)?.Name == channel)
            {
                DataBase.AddChannel(channel);
                LastMessages = LastMessagesHelper.FillDictionary();
                Prefixes = PrefixHelper.FillDictionary();
                EmoteInFront = EmoteInFrontHelper.FillDictionary();
                try
                {
                    TwitchClient.JoinChannel(channel);
                    Send(channel, "I'm online");
                    responseMessage = $"successfully joined #{channel}";
                    return true;
                }
                catch (Exception)
                {
                    responseMessage = $"unable to join #{channel}";
                    return false;
                }
            }
            else
            {
                responseMessage = $"channel #{channel} does not exist";
                return false;
            }
        }

        #region SystemInfo

        public string GetSystemInfo()
        {
            return $"Uptime: {Runtime} || Memory usage: {GetMemoryUsage()}MB || Executed commands: {CommandCount}";
        }

        private static double GetMemoryUsage()
        {
            return Math.Truncate(Process.GetCurrentProcess().PrivateMemorySize64 / Math.Pow(10, 6) * 100) / 100;
        }

        #endregion SystemInfo

        #region Bot_On

        private void Client_OnLog(object sender, OnLogArgs e)
        {
            //ConsoleOut($"LOG>{e.Data}");
        }

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            ConsoleOut("BOT>CONNECTED", true, ConsoleColor.Red);
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            ConsoleOut($"BOT>Joined channel: {e.Channel}", fontColor: ConsoleColor.Red);
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            if (!MessageHelper.IsSpecialUser(e.ChatMessage.Username))
            {
                new MessageHandler(this, e.ChatMessage).Handle();
            }
            ConsoleOut($"#{e.ChatMessage.Channel}>{e.ChatMessage.Username}: {e.ChatMessage.GetMessage()}");
        }

        private void Client_OnMessageSent(object sender, OnMessageSentArgs e)
        {
            ConsoleOut($"#{e.SentMessage.Channel}>{Resources.Username}: {e.SentMessage.Message}", fontColor: ConsoleColor.Green);
        }

        private void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            new WhisperHandler(this, e.WhisperMessage).Handle();
            ConsoleOut($"WHISPER>{e.WhisperMessage.Username}: {e.WhisperMessage.Message}");
        }

        private void Client_OnConnectionError(object sender, OnConnectionErrorArgs e)
        {
            ConsoleOut($"CONNECTION-ERROR>{e.Error.Message}", true, ConsoleColor.Red);
            Restart();
        }

        private void Client_OnError(object sender, OnErrorEventArgs e)
        {
            ConsoleOut($"ERROR>{e.Exception.Message}", true, ConsoleColor.Red);
            Restart();
        }

        private void Client_OnDisconnect(object sender, OnDisconnectedEventArgs e)
        {
            ConsoleOut($"BOT>DISCONNECTED", true, ConsoleColor.Red);
            Restart();
        }

        private void Client_OnReconnected(object sender, OnReconnectedEventArgs e)
        {
            ConsoleOut($"BOT>RECONNECTED", true, ConsoleColor.Red);
        }

        private void Client_OnUserJoinedChannel(object sender, OnUserJoinedArgs e)
        {
            if (e.Channel == Resources.SecretOfflineChat && !new JsonController().BotData.UserLists.SecretUsers.Contains(e.Username))
            {
                Send(Resources.SecretOfflineChat, $"{e.Username} joined the chat");
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
            Bot.Timers.GetTimer(1000).Elapsed += OnTimer1000;
            Bot.Timers.GetTimer(30000).Elapsed += OnTimer30000;
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

        private void OnTimer10Days(object sender, ElapsedEventArgs e)
        {
            TimerFunctions.TwitchApiRefreshAccessToken();
        }

        #endregion Timer
    }
}
