#nullable disable

using OkayegTeaTimeCSharp.Time;

namespace OkayegTeaTimeCSharp.Database.Models
{
    public partial class Spotify
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public long Time { get; set; } = TimeHelper.Now();

        public Spotify(string username, string accessToken, string refreshToken)
        {
            Username = username;
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }

        public Spotify(int id, string username, string accessToken, string refreshToken, long time)
        {
            Id = id;
            Username = username;
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            Time = time;
        }
    }
}