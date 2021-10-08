using OkayegTeaTimeCSharp.JsonData.JsonClasses.HttpRequests;

namespace OkayegTeaTimeCSharp.Twitch.Bot.EmoteManagementNotifications
{
    public class NotificatorChannel
    {
        public string Name { get; }

        public List<SevenTvEmote> New7TVEmotes { get; set; }

        public List<SevenTvEmote> Old7TVEmotes { get; set; }

        public List<BttvSharedEmote> NewBTTVEmotes { get; set; }

        public List<BttvSharedEmote> OldBTTVEmotes { get; set; }

        public List<FfzEmote> NewFFZEmotes { get; set; }

        public List<FfzEmote> OldFFZEmotes { get; set; }

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
