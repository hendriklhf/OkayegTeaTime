using HLE;

#nullable disable

namespace OkayegTeaTime.Database.EntityFrameworkModels
{
    public class Channel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public byte[] EmoteInFront { get; set; }
        public byte[] Prefix { get; set; }
        public bool? EmoteManagementSub { get; set; }

        public Channel(long id, string name, string emote = null, string prefix = null, bool emoteManagementSub = false)
        {
            Id = id;
            Name = name;
            EmoteInFront = emote?.Encode();
            Prefix = prefix?.Encode();
            EmoteManagementSub = emoteManagementSub;
        }

        public Channel(long id, string name, byte[] emoteInFront, byte[] prefix, bool? emoteManagementSub = false)
        {
            Id = id;
            Name = name;
            EmoteInFront = emoteInFront;
            Prefix = prefix;
            EmoteManagementSub = emoteManagementSub;
        }
    }
}
