#nullable disable

namespace OkayegTeaTimeCSharp.Database.Models
{
    public partial class Spotify
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public long Time { get; set; }
    }
}