namespace OkayegTeaTime.Files.JsonClasses.Settings;

public class Settings
{
    public Twitch Twitch { get; set; }

    public Discord Discord { get; set; }

    public List<string> SecretOfflineChatEmotes { get; set; }

    public Spotify Spotify { get; set; }

    public UserLists UserLists { get; set; }

    public string RepositoryUrl { get; set; }

    public string ChatterinoChar { get; set; }

    public string DebugChannel { get; set; }

    public string SecretOfflineChatChannel { get; set; }

    public List<string> NotLoggedChannels { get; set; }

    public DbConnection DatabaseConnection { get; set; }
}
