#nullable disable

namespace OkayegTeaTimeCSharp.Database.Models
{
    public partial class Suggestion
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public byte[] Suggestion1 { get; set; }
        public string Channel { get; set; }
        public long Time { get; set; }
        public bool? Done { get; set; }
    }
}
