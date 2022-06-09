#nullable disable

namespace OkayegTeaTime.Database.EntityFrameworkModels
{
    public class SubEmoteNotification
    {
        public int Id { get; set; }
        public int ChannelId { get; set; }
        public string SubChannel { get; set; }
    }
}
