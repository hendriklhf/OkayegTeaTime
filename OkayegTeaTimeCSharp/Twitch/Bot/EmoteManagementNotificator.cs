using HLE.Time;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;

namespace OkayegTeaTimeCSharp.Twitch.Bot
{
    public class EmoteManagementNotificator
    {
        public List<string> Channels { get; set; }

        private readonly Timer _timer = new(new Minute().Milliseconds);

        public EmoteManagementNotificator(List<string> channels)
        {
            Channels = channels;
            _timer.Elapsed += Timer_OnElapsed;
            _timer.Start();
        }

        public void LoadEmotes()
        {
            Task.Run(async () => await LoadEmotesAsync());
        }

        private async Task LoadEmotesAsync()
        {
            Channels.ForEach(c =>
            {
                Task.Run(async () =>
                {
                    //loading emotes into db
                    await Task.Delay(1000);
                });
            });
        }

        public void DetectChange()
        {
        }

        public void NotifyChannel()
        {
        }

        private void Timer_OnElapsed(object sender, ElapsedEventArgs e)
        {
            LoadEmotes();
            DetectChange();
            NotifyChannel();
        }
    }
}
