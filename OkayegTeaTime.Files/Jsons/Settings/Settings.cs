#nullable disable

namespace OkayegTeaTime.Files.Jsons.Settings;

public class Settings
{
    public Twitch Twitch { get; set; }

    public Discord Discord { get; set; }

    public string[] OfflineChatEmotes { get; set; }

    public Spotify Spotify { get; set; }

    public UserLists UserLists { get; set; }

    public string RepositoryUrl { get; set; }

    public string OfflineChatChannel { get; set; }

    public DbConnection DatabaseConnection { get; set; }

    public string OpenWeatherMapApiKey { get; set; }
}
