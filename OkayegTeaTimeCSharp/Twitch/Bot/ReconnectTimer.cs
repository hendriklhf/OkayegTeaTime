using System.Timers;

namespace OkayegTeaTimeCSharp.Twitch.Bot
{
    public class ReconnectTimer
    {
        public TwitchBot TwitchBot { get; }

        public Timer Timer { get; set; }

        public int Loops { get; set; }

        private readonly int _interval = 30000;

        public ReconnectTimer(TwitchBot twitchBot)
        {
            TwitchBot = twitchBot;
            Timer = new(_interval);
            Timer.Elapsed += OnElapsed;
        }

        public void Start()
        {
            Timer.Start();
        }

        private void OnElapsed(object sender, ElapsedEventArgs e)
        {
            TwitchBot.TwitchClient.Reconnect();
            TwitchBot.WebSocketClient.Reconnect();
            Timer.Stop();
        }
    }
}
