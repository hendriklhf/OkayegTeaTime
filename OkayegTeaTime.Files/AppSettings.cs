using System;
using System.Reflection;
using OkayegTeaTime.Files.Jsons.Settings;

#nullable disable

namespace OkayegTeaTime.Files;

public static class AppSettings
{
    public static string AssemblyName { get; } = Assembly.GetEntryAssembly()?.GetName().Name ?? throw new ArgumentNullException($"{nameof(AssemblyName)} can not be null.");

    public static string ChatterinoChar => "\uDB40\uDC00";

    public static DbConnection DbConnection => JsonController.GetSettings().DatabaseConnection;

    public static Discord Discord => JsonController.GetSettings().Discord;

    public static string RepositoryUrl => JsonController.GetSettings().RepositoryUrl;

    public static string OfflineChatChannel => JsonController.GetSettings().OfflineChatChannel;

    public static string[] OfflineChatEmotes => JsonController.GetSettings().OfflineChatEmotes;

    public static string OpenWeatherMapApiKey => JsonController.GetSettings().OpenWeatherMapApiKey;

    public static Spotify Spotify => JsonController.GetSettings().Spotify;

    public static Twitch Twitch => JsonController.GetSettings().Twitch;

    public static UserLists UserLists => JsonController.GetSettings().UserLists;

    public const short AfkCooldown = 10000;
    public const string DefaultEmote = "Okayeg";
    public const byte MaxEmoteInFrontLength = 20;
    public const short MaxMessageLength = 500;
    public const byte MaxPrefixLength = 10;
    public const byte MaxReminders = 10;
    public const short DelayBetweenSentMessages = 1300;
    public const string Suffix = "eg";
    public const string SettingsFileName = "Settings.json";
    public const string FfzSetIdReplacement = "mainSet";
}
