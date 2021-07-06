using OkayegTeaTimeCSharp.Utils;

#nullable enable

namespace OkayegTeaTimeCSharp.Database.Models
{
    public partial class EmoteInFront
    {
        public int Id { get; set; }
        public string Channel { get; set; }
        public byte[]? Emote { get; set; }

        public EmoteInFront(string channel, byte[]? emote)
        {
            Channel = $"#{channel.RemoveHashtag()}";
            Emote = emote;
        }
    }
}
