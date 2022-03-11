#nullable disable

namespace OkayegTeaTime.Database.Models
{
    public class SubEmoteNotification
    {
        public int Id { get; set; }
        public int ChannelId { get; set; }
        public string SubChannel { get; set; }
    }
}
