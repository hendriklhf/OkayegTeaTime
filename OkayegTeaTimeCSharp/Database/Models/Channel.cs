#nullable disable

namespace OkayegTeaTimeCSharp.Database.Models
{
    public partial class Channel
    {
        public int Id { get; set; }
        public int? User { get; set; }
        public byte[] EmoteInFront { get; set; }
        public byte[] Prefix { get; set; }

        public virtual User UserNavigation { get; set; }
    }
}
