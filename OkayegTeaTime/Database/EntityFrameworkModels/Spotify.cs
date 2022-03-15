using HLE.Time;

#nullable disable

namespace OkayegTeaTime.Database.EntityFrameworkModels
{
    public class Spotify
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public long Time { get; set; } = TimeHelper.Now();
        public bool? SongRequestEnabled { get; set; } = false;

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
