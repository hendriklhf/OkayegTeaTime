#nullable disable

namespace OkayegTeaTimeCSharp.Database.Models
{
    public partial class Prefix
    {
        public int Id { get; set; }
        public string Channel { get; set; }
        public string PrefixString { get; set; }

        public Prefix(int id, string channel, string prefixString)
        {
            Id = id;
            Channel = channel;
            PrefixString = prefixString;
        }

        public Prefix(string channel, string prefixString)
        {
            Channel = $"#{channel.Replace("#", "")}";
            PrefixString = prefixString;
        }
    }
}
