using OkayegTeaTimeCSharp.Database;
using OkayegTeaTimeCSharp.Messages;
using OkayegTeaTimeCSharp.Properties;
using OkayegTeaTimeCSharp.Time;
using OkayegTeaTimeCSharp.Utils;
using OkayegTeaTimeCSharp.Whisper;
using System;
using System.Collections.Generic;
using System.Threading;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Enums;
using TwitchLib.Communication.Models;
using static OkayegTeaTimeCSharp.Program;
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

        public DottedNumber CommandCount
        {
            get => _commandCount;
            set => _commandCount++;
        }

        public string Runtime => ConvertMillisecondsToPassedTime(_runtime);

        private readonly long _runtime;

        private DottedNumber _commandCount = new(1);

        private static TwitchBot _okayegTeaTime;

        public static readonly List<Timers::Timer> ListTimer = new();

        public static readonly Dictionary<string, string> LastMessages = new();

        public static readonly List<Cooldown> Cooldowns = new();

        public static readonly List<AfkCooldown> AfkCooldowns = new();

        public TwitchBot()
        {
            _runtime = Now();

            ConnectionCredentials = new(Resources.Username, Resources.OAuthToken);
            ClientOptions = new()
            {
                ClientType = ClientType.Chat,
                ReconnectionPolicy = new(5000, 10000, 1000),
            };
            WebSocketClient = new(ClientOptions);
            TwitchClient = new(WebSocketClient);

            TwitchClient.Initialize(ConnectionCredentials, Config.GetChannels());

            //TwitchClient.OnLog += Client_OnLog;
            TwitchClient.OnConnected += Client_OnConnected;
            TwitchClient.OnJoinedChannel += Client_OnJoinedChannel;
            TwitchClient.OnMessageReceived += Client_OnMessageReceived;
            TwitchClient.OnMessageSent += Client_OnMessageSent;
            TwitchClient.OnWhisperReceived += Client_OnWhisperReceived;

            TwitchClient.Connect();

            InitializeTimers();
        }

        public void SetBot()
        {
            _okayegTeaTime ??= this;
        }

        public void Send(string channel, string message, string emoteInFront = Config.EmoteInFront)
        {
            if (!Config.GetNotAllowedChannels().Contains(channel.ReplaceHashtag()))
            {
                if ($"{emoteInFront} {message} {Resources.ChatterinoChar}".Length < 495)
                {
                    message = LastMessages[$"#{channel.ReplaceHashtag()}"] == message ? $"{message} {Resources.ChatterinoChar}" : message;
                    TwitchClient.SendMessage(channel.ReplaceHashtag(), $"{emoteInFront} {message}");
                    LastMessages[$"#{channel.ReplaceHashtag()}"] = message;
                }
                else
                {
                    new DividedMessage(this, channel, message).StartSending();
                }
            }
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

        private void Client_OnLog(object sender, OnLogArgs e)
        {
            ConsoleOut($"LOG: {e.Data}");
        }

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            ConsoleOut("BOT>CONNECTED");
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            ConsoleOut($"BOT>Joined channel: {e.Channel}");
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            Thread thread = new(OnMessage);
            thread.Start(e.ChatMessage);

            ConsoleOut($"#{e.ChatMessage.Channel}>{e.ChatMessage.Username}: {e.ChatMessage.GetMessage()}");
        }

        private void Client_OnMessageSent(object sender, OnMessageSentArgs e)
        {
            ConsoleOut($"#{e.SentMessage.Channel}>{Resources.Username}: {e.SentMessage.Message}");
        }

        private void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            WhisperHandler.Handle(this, e.WhisperMessage);

            ConsoleOut($"WHISPER>{e.WhisperMessage.Username}: {e.WhisperMessage.Message}");
        }

        #endregion Bot_On

        #region Threading

        private static void OnMessage(object chatMessage)
        {
            if (!MessageHelper.IsSpecialUser(((ChatMessage)chatMessage).Username))
            {
                MessageHandler.Handle(_okayegTeaTime, (ChatMessage)chatMessage);
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
            ListTimer.ForEach(timer => timer.Start());
        }

        private static void AddTimerFunction()
        {
            Timers.GetTimer(1000).Elapsed += OnTimer1000;
            Timers.GetTimer(30000).Elapsed += OnTimer30000;
            Timers.GetTimer(new Day(10).ToMilliseconds()).Elapsed += OnTimer10Days;
        }

        private static void OnTimer1000(object sender, Timers::ElapsedEventArgs e)
        {
            TimerFunctions.CheckForTimedReminders(_okayegTeaTime);
        }

        private static void OnTimer30000(object sender, Timers::ElapsedEventArgs e)
        {
            TimerFunctions.BanSecretChatUsers(_okayegTeaTime);
        }

        private static void OnTimer10Days(object sender, Timers::ElapsedEventArgs e)
        {
            TimerFunctions.TwitchApiRefreshAccessToken();
        }

        #endregion Timer
    }
}