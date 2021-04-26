#nullable disable

namespace OkayegTeaTimeCSharp.Database.Models
{
    public partial class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public byte[] MessageText { get; set; }
        public string Type { get; set; }
        public long? Time { get; set; }
        public string IsAfk { get; set; }
        public long? Egs { get; set; }
    }
}
