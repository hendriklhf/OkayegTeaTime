#pragma warning disable CS0659

using OkayegTeaTimeCSharp.Models;
using System.Collections.Generic;

namespace OkayegTeaTimeCSharp.Twitch.Bot.EmoteManagementNotifications
{
    public class NotificatorChannel
    {
        public string Name { get; }

        public List<Emote> New7TVEmotes { get; set; }

        public List<Emote> Old7TVEmotes { get; set; }

        public List<Emote> NewBTTVEmotes { get; set; }

        public List<Emote> OldBTTVEmotes { get; set; }

        public List<Emote> NewFFZEmotes { get; set; }

        public List<Emote> OldFFZEmotes { get; set; }

        public NotificatorChannel(string name)
        {
            Name = name;
        }

        public override bool Equals(object obj)
        {
            return obj is NotificatorChannel notificatorChannel && notificatorChannel.Name == Name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
