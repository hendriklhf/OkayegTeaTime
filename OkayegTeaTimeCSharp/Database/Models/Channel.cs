using HLE.Strings;

#nullable disable

namespace OkayegTeaTimeCSharp.Database.Models
{
    public partial class Channel
    {
        public int Id { get; set; }
        public string ChannelName { get; set; }
        public byte[] EmoteInFront { get; set; }
        public byte[] Prefix { get; set; }

        public Channel(string channel, string emote = null, string prefix = null)
        {
            ChannelName = channel;
            EmoteInFront = emote?.Encode();
            Prefix = prefix?.Encode();
        }

        public Channel(int id, string channelName, byte[] emoteInFront, byte[] prefix)
        {
            Id = id;
            ChannelName = channelName;
            EmoteInFront = emoteInFront;
            Prefix = prefix;
        }
    }
}
