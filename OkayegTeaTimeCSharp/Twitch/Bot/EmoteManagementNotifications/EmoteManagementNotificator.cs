using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using HLE.Collections;
using HLE.Enums;
using HLE.Strings;
using HLE.Time;
using OkayegTeaTimeCSharp.Database;
using OkayegTeaTimeCSharp.HttpRequests;
using OkayegTeaTimeCSharp.Logging;
using OkayegTeaTimeCSharp.Models;
using OkayegTeaTimeCSharp.Twitch.Bot.EmoteManagementNotifications.Enums;

namespace OkayegTeaTimeCSharp.Twitch.Bot.EmoteManagementNotifications
{
    public class EmoteManagementNotificator
    {
        public TwitchBot TwitchBot { get; }

        private readonly List<NotificatorChannel> _channels = new();
        private readonly Timer _timer = new(new Minute().Milliseconds);

        public EmoteManagementNotificator(TwitchBot twitchBot)
        {
            TwitchBot = twitchBot;
            DataBase.GetEmoteManagementSubs().ForEach(c => _channels.Add(new(c)));
            InitChannels();
            _timer.Elapsed += Timer_OnElapsed;
            _timer.Start();
        }

        private void InitChannels()
        {
            _channels.Where(c => AreEmoteListsNull(c)).ForEach(c =>
            {
                List<Emote> emotes;
                do
                {
                    try
                    {
                        emotes = HttpRequest.Get7TVEmotes(c.Name);
                        c.New7TVEmotes = emotes;
                        c.Old7TVEmotes = emotes;
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
                while (c.New7TVEmotes is null || c.Old7TVEmotes is null);

                do
                {
                    try
                    {
                        emotes = HttpRequest.GetBTTVEmotes(c.Name);
                        c.NewBTTVEmotes = emotes;
                        c.OldBTTVEmotes = emotes;
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
                while (c.NewBTTVEmotes is null || c.OldBTTVEmotes is null);

                do
                {
                    try
                    {
                        emotes = HttpRequest.GetFFZEmotes(c.Name);
                        c.NewFFZEmotes = emotes;
                        c.OldFFZEmotes = emotes;
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
                while (c.NewFFZEmotes is null || c.OldFFZEmotes is null);
            });
        }

        private void LoadEmotes()
        {
            _channels.ForEach(c =>
            {
                try
                {
                    c.Old7TVEmotes = c.New7TVEmotes;
                    c.OldBTTVEmotes = c.NewBTTVEmotes;
                    c.OldFFZEmotes = c.OldFFZEmotes;

                    c.New7TVEmotes = HttpRequest.Get7TVEmotes(c.Name);
                    c.NewBTTVEmotes = HttpRequest.GetBTTVEmotes(c.Name);
                    c.NewFFZEmotes = HttpRequest.GetFFZEmotes(c.Name);
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            });
        }

        private void DetectChange()
        {
            _channels.ForEach(c =>
            {
                List<Emote> new7TVEmotes = c.New7TVEmotes?.Where(e => c.Old7TVEmotes?.Contains(e) == false).ToList();
                NotifyChannel(c.Name, new7TVEmotes, NotificationType.NewEmote);

                List<Emote> newBTTVEmotes = c.NewBTTVEmotes?.Where(e => c.OldBTTVEmotes?.Contains(e) == false).ToList();
                NotifyChannel(c.Name, newBTTVEmotes, NotificationType.NewEmote);

                List<Emote> newFFZEmotes = c.NewFFZEmotes?.Where(e => c.OldFFZEmotes?.Contains(e) == false).ToList();
                NotifyChannel(c.Name, newFFZEmotes, NotificationType.NewEmote);
            });
        }

        private void NotifyChannel(string channel, List<Emote> emotes, NotificationType type)
        {
            if (type == NotificationType.NewEmote && !emotes.IsNullOrEmpty())
            {
                TwitchBot.Send(channel, $"Newly added emote{(emotes.Count > 1 ? "s" : string.Empty)}: {emotes.Select(e => e.Name).ToSequence()}");
            }
        }

        private void Timer_OnElapsed(object sender, ElapsedEventArgs e)
        {
            LoadEmotes();
            DetectChange();
        }

        private bool AreEmoteListsNull(NotificatorChannel channel)
        {
            return channel.New7TVEmotes is null || channel.Old7TVEmotes is null
                || channel.NewBTTVEmotes is null || channel.OldBTTVEmotes is null
                || channel.NewFFZEmotes is null || channel.OldFFZEmotes is null;
        }

        public void AddChannel(string channel)
        {
            if (!_channels.Any(c => c.Name == channel))
            {
                _channels.Add(new(channel));
                InitChannels();
            }
        }

        public void RemoveChannel(string channel)
        {
            _channels.Remove(new(channel));
        }
    }
}
