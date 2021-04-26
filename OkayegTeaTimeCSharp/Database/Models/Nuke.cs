#nullable disable

namespace OkayegTeaTimeCSharp.Database.Models
{
    public partial class Nuke
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Channel { get; set; }
        public byte[] Word { get; set; }
        public long? TimeoutTime { get; set; }
        public long? ForTime { get; set; }
    }
}
