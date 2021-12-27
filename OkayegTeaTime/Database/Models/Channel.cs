using HLE.Strings;

#nullable disable

namespace OkayegTeaTime.Database.Models
{
    public class Channel
    {
        public int Id { get; set; }
        public string ChannelName { get; set; }
        public byte[] EmoteInFront { get; set; }
        public byte[] Prefix { get; set; }
        public bool? EmoteManagementSub { get; set; } = false;

        public Channel(string channel, string emote = null, string prefix = null)
        {
            ChannelName = channel;
            EmoteInFront = emote?.Encode();
            Prefix = prefix?.Encode();
        }

        public Channel(int id, string channelName, byte[] emoteInFront, byte[] prefix, bool? emoteManagementSub = false)
        {
            Id = id;
            ChannelName = channelName;
            EmoteInFront = emoteInFront;
            Prefix = prefix;
            EmoteManagementSub = emoteManagementSub;
        }
    }
}
