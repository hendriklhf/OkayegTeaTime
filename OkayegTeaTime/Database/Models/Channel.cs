using HLE.Strings;

#nullable disable

namespace OkayegTeaTime.Database.Models
{
    public class Channel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] EmoteInFront { get; set; }
        public byte[] Prefix { get; set; }
        public bool? EmoteManagementSub { get; set; }

        public Channel(int id, string name, string emote = null, string prefix = null, bool emoteManagementSub = false)
        {
            Id = id;
            Name = name;
            EmoteInFront = emote?.Encode();
            Prefix = prefix?.Encode();
            EmoteManagementSub = emoteManagementSub;
        }

        public Channel(int id, string name, byte[] emoteInFront, byte[] prefix, bool? emoteManagementSub = false)
        {
            Id = id;
            Name = name;
            EmoteInFront = emoteInFront;
            Prefix = prefix;
            EmoteManagementSub = emoteManagementSub;
        }
    }
}
