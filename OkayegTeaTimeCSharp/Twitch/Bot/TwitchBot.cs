using OkayegTeaTimeCSharp.Database;
using OkayegTeaTimeCSharp.Messages;
using OkayegTeaTimeCSharp.Properties;
using OkayegTeaTimeCSharp.Utils;
using OkayegTeaTimeCSharp.Whisper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Enums;
using TwitchLib.Communication.Events;
using TwitchLib.Communication.Models;
using static OkayegTeaTimeCSharp.Time.TimeHelper;
using Timers = System.Timers;

namespace OkayegTeaTimeCSharp.Twitch.Bot
{
    public class TwitchBot
    {
        public TwitchClient TwitchClient { get; private set; }

        public ConnectionCredentials ConnectionCredentials { get; private set; }

        public ClientOptions ClientOptions { get; private set; }

        public WebSocketClient WebSocketClient { get; private set; }

        public string CommandCount
        {
            get => new DottedNumber(_commandCount).Number;
            set => _commandCount++;
        }

        public string Runtime => ConvertMillisecondsToPassedTime(_runtime);

        private readonly long _runtime;

        private long _commandCount = 1;

        private static TwitchBot _okayegTeaTime;

        public static List<Timers::Timer> ListTimer = new();

        public static readonly Dictionary<string, string> LastMessages = new();

        public static readonly List<Cooldown> Cooldowns = new();

        public static readonly List<AfkCooldown> AfkCooldowns = new();

        public TwitchBot(string[] args)
        {
            _runtime = Now();

            ConnectionCredentials = new(Resources.Username, Resources.OAuthToken);
            ClientOptions = new()
            {
                ClientType = ClientType.Chat,
                //SendDelay = 10,
                ReconnectionPolicy = new(5000),
                DisconnectWait = 5000,
                SendCacheItemTimeout = TimeSpan.FromSeconds(1.3),
            };
            WebSocketClient = new(ClientOptions);
            TwitchClient = new(WebSocketClient);

            if (args.Any(param => param.ToLower() == "test" || param.ToLower() == "debug"))
            {
                TwitchClient.Initialize(ConnectionCredentials, "xxdirkthecrafterxx");
                //LastMessages.Add("#lbnshlfe", "");
            }
            else
            {
                TwitchClient.Initialize(ConnectionCredentials, Config.GetChannels());
            }

            //TwitchClient.OnLog += Client_OnLog;
            TwitchClient.OnConnected += Client_OnConnected;
            TwitchClient.OnJoinedChannel += Client_OnJoinedChannel;
            TwitchClient.OnMessageReceived += Client_OnMessageReceived;
            TwitchClient.OnWhisperReceived += Client_OnWhisperReceived;
            TwitchClient.OnError += Client_OnError;
            TwitchClient.OnConnectionError += Client_OnConnectionError;
            TwitchClient.OnReconnected += Client_OnReconnected;
            WebSocketClient.OnReconnected += WebSocketClient_OnReconnected;

            TwitchClient.Connect();

            InitializeTimers();
        }

        public void SetBot()
        {
            _okayegTeaTime ??= this;
        }

        public void Send(string channel, string message, string emoteInFront = "Okayeg")
        {
            message = LastMessages[$"#{channel.ReplaceHashtag()}"] == message ? $"{message} {Resources.ChatterinoChar}" : message;
            TwitchClient.SendMessage(channel.ReplaceHashtag(), $"{emoteInFront} {message}");
            LastMessages[$"#{channel.ReplaceHashtag()}"] = message;
        }

        public void JoinChannel(string channel)
        {
            DataBase.AddChannel(channel);
            LastMessages.Add($"#{channel.ReplaceHashtag()}", "");
            PrefixHelper.FillDictionary();
            try
            {
                TwitchClient.JoinChannel(channel.ReplaceHashtag());
                Send(channel, "/ I'm online");
            }
            catch (Exception)
            {
                Send(Resources.Username, $"{Resources.Owner}, unable to join #{channel.ReplaceHashtag()}");
            }
        }

        #region Bot_On

        //private void Client_OnLog(object sender, OnLogArgs e)
        //{
        //    Console.WriteLine($"LOG: {e.Data}");
        //}

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            Console.WriteLine("BOT>CONNECTED");
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            Console.WriteLine($"BOT>Joined channel: {e.Channel}");
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            Thread thread = new(OnMessage);
            thread.Start(e.ChatMessage);

            Console.WriteLine($"#{e.ChatMessage.Channel}>{e.ChatMessage.Username}: {e.ChatMessage.GetMessage()}");
        }

        private void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            WhisperHandler.Handle(_okayegTeaTime, e.WhisperMessage);

            Console.WriteLine($"WHISPER>{e.WhisperMessage.Username}: {e.WhisperMessage.Message}");
        }

        private void Client_OnError(object sender, OnErrorEventArgs e)
        {
            Console.WriteLine($"TwitchClient>Exception: {e.Exception.Message}");
            TwitchClient.Reconnect();
            WebSocketClient.Reconnect();
        }

        private void Client_OnConnectionError(object sender, OnConnectionErrorArgs e)
        {
            Console.WriteLine($"TwitchClient>Exception: {e.Error.Message}");
            TwitchClient.Reconnect();
            WebSocketClient.Reconnect();
        }

        private void Client_OnReconnected(object sender, OnReconnectedEventArgs e)
        {
            Console.WriteLine("TwitchClient>Reconnected!");
        }

        private void WebSocketClient_OnReconnected(object sender, OnReconnectedEventArgs e)
        {
            Console.WriteLine("WebSocketClient>Reconnected!");
        }

        #endregion Bot_On

        #region Threading

        private static void OnMessage(object chatMessage)
        {
            if (!MessageHelper.IsSpecialUser(((ChatMessage)chatMessage).Username))
            {
                MessageHandler.Handle(_okayegTeaTime, ((ChatMessage)chatMessage));
            }
        }

        #endregion Threading

        #region Timer

        private static void InitializeTimers()
        {
            Timers.CreateTimers();
            AddTimerFunction();
            StartTimers();
        }

        private static void StartTimers()
        {
            ListTimer.ForEach(timer => timer.Enabled = true);
        }

        private static void AddTimerFunction()
        {
            Timers.GetTimer(1000).Elapsed += OnTimer1000;
            Timers.GetTimer(30000).Elapsed += OnTimer30000;
        }

        private static void OnTimer1000(object sender, Timers::ElapsedEventArgs e)
        {
            TimerFunctions.CheckForTimedReminders(_okayegTeaTime);
        }

        private static void OnTimer30000(object sender, Timers::ElapsedEventArgs e)
        {
            TimerFunctions.BanSecretChatUsers(_okayegTeaTime);
        }

        #endregion Timer
    }
}