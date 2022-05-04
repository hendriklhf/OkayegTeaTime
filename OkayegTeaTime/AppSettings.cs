using System.Reflection;
using OkayegTeaTime.Files;
using OkayegTeaTime.Files.Jsons.Settings;

#nullable disable

namespace OkayegTeaTime;

public static class AppSettings
{
    public static string AssemblyName { get; } = Assembly.GetExecutingAssembly().GetName().Name;

    public static string ChatterinoChar => "\uDB40\uDC00";

    public static DbConnection DbConnection => JsonController.GetSettings().DatabaseConnection;

    public static string DebugChannel => JsonController.GetSettings().DebugChannel;

    public static Files.Jsons.Settings.Discord Discord => JsonController.GetSettings().Discord;

    public static string RepositoryUrl => JsonController.GetSettings().RepositoryUrl;

    public static string OfflineChatChannel => JsonController.GetSettings().OfflineChatChannel;

    public static List<string> OfflineChatEmotes => JsonController.GetSettings().OfflineChatEmotes;

    public static Files.Jsons.Settings.Spotify Spotify => JsonController.GetSettings().Spotify;

    public static Files.Jsons.Settings.Twitch Twitch => JsonController.GetSettings().Twitch;

    public static UserLists UserLists => JsonController.GetSettings().UserLists;

    public const short AfkCooldown = 10000;
    public const string DefaultEmote = "Okayeg";
    public const byte MaxEmoteInFrontLength = 20;
    public const short MaxMessageLength = 500;
    public const byte MaxPrefixLength = 10;
    public const byte MaxReminders = 10;
    public const short DelayBetweenSentMessages = 1300;
    public const short DelayBetweenReceivedMessages = 500;
    public const string Suffix = "eg";
    public const string SettingsFileName = "Settings.json";
}
