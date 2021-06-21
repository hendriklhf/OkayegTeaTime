#nullable enable

using OkayegTeaTimeCSharp.Utils;

namespace OkayegTeaTimeCSharp.Database.Models
{
    public partial class Prefix
    {
        public int Id { get; set; }
        public string Channel { get; set; }
        public byte[]? PrefixString { get; set; }

        public Prefix(int id, string channel, byte[] prefixString)
        {
            Id = id;
            Channel = channel;
            PrefixString = prefixString;
        }

        public Prefix(string channel, byte[] prefixString)
        {
            Channel = $"#{channel.ReplaceHashtag()}";
            PrefixString = prefixString;
        }
    }
}