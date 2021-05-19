#nullable disable

namespace OkayegTeaTimeCSharp.Database.Models
{
    public partial class Bot
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Oauth { get; set; }
        public string Channels { get; set; }
    }
}