#nullable disable

using System.Reflection;
using OkayegTeaTime.Files;
using OkayegTeaTime.Files.JsonClasses.CommandData;
using JsonSettings = OkayegTeaTime.Files.JsonClasses.Settings;

namespace OkayegTeaTime;

public static class AppSettings
{
    public static string AssemblyName { get; } = Assembly.GetExecutingAssembly().GetName().Name;

    public static string ChatterinoChar => JsonController.Settings.ChatterinoChar;

    public static CommandList CommandList => JsonController.CommandList;

    public static JsonSettings::DbConnection DbConnection => JsonController.Settings.DatabaseConnection;

    public static string DebugChannel => JsonController.Settings.DebugChannel;

    public static JsonSettings::Discord Discord => JsonController.Settings.Discord;

    public static List<string> NotLoggedChannels => JsonController.Settings.NotLoggedChannels;

    public static List<string> RandomWords => JsonController.RandomWords;

    public static string RepositoryUrl => JsonController.Settings.RepositoryUrl;

    public static string SecretOfflineChatChannel => JsonController.Settings.SecretOfflineChatChannel;

    public static List<string> SecretOfflineChatEmotes => JsonController.Settings.SecretOfflineChatEmotes;

    public static JsonSettings::Spotify Spotify => JsonController.Settings.Spotify;

    public static JsonSettings::Twitch Twitch => JsonController.Settings.Twitch;

    public static JsonSettings::UserLists UserLists => JsonController.Settings.UserLists;

    public const short AfkCooldown = 10000;
    public const string DefaultEmote = "Okayeg";
    public const byte MaxEmoteInFrontLength = 20;
    public const short MaxMessageLength = 500;
    public const byte MaxPrefixLength = 10;
    public const byte MaxReminders = 10;
    public const short MinDelayBetweenMessages = 1300;
    public const string Suffix = "eg";
}
