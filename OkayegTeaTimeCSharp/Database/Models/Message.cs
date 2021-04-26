#nullable disable

namespace OkayegTeaTimeCSharp.Database.Models
{
    public partial class Message
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public byte[] MessageText { get; set; }
        public string Channel { get; set; }
        public long? Time { get; set; }
    }
}
