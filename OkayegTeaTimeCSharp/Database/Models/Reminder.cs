#nullable disable

namespace OkayegTeaTimeCSharp.Database.Models
{
    public partial class Reminder
    {
        public int Id { get; set; }
        public string FromUser { get; set; }
        public string ToUser { get; set; }
        public byte[] Message { get; set; }
        public string Channel { get; set; }
        public long Time { get; set; }
        public long ToTime { get; set; }
    }
}
