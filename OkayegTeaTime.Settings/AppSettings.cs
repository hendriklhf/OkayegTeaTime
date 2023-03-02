﻿using System;
using System.Reflection;
using OkayegTeaTime.Models.Json;
using UserLists = OkayegTeaTime.Settings.Models.UserLists;

#nullable disable

namespace OkayegTeaTime.Settings;

public static class AppSettings
{
    public static string AssemblyName { get; } = Assembly.GetEntryAssembly()?.GetName().Name ?? throw new ArgumentNullException($"{nameof(AssemblyName)} can not be null.");

    public static DatabaseConnection DatabaseConnection { get; private set; }

    public static DiscordSettings Discord { get; private set; }

    public static string RepositoryUrl { get; private set; }

    public static string OfflineChatChannel { get; private set; }

    public static string[] OfflineChatEmotes { get; private set; }

    public static string OpenWeatherMapApiKey { get; private set; }

    public static SpotifySettings Spotify { get; private set; }

    public static TwitchSettings Twitch { get; private set; }

    public static UserLists UserLists { get; private set; }

    public static GachiSong[] GachiSongs { get; private set; }

    public static CommandList CommandList { get; private set; }

    public const short AfkCooldown = 10000;
    public const string DefaultEmote = "Okayeg";
    public const byte MaxEmoteInFrontLength = 20;
    public const short MaxMessageLength = 500;
    public const byte MaxPrefixLength = 10;
    public const byte MaxReminders = 10;
    public const short DelayBetweenSentMessages = 1300;
    public const string Suffix = "eg";
    public const string SettingsFileName = "Settings.json";
    public const string ChatterinoChar = "\uDB40\uDC00";
    public const string HleNugetVersionId = "107874";

    public static void Initialize()
    {
        OkayegTeaTime.Models.Json.Settings settings = JsonController.GetSettings();
        DatabaseConnection = settings.DatabaseConnection;
        Discord = settings.Discord;
        RepositoryUrl = settings.RepositoryUrl;
        OfflineChatChannel = settings.OfflineChatChannel;
        OfflineChatEmotes = settings.OfflineChatEmotes;
        OpenWeatherMapApiKey = settings.OpenWeatherMapApiKey;
        Spotify = settings.Spotify;
        Twitch = settings.Twitch;
        UserLists = new(settings.UserLists);

        GachiSongs = JsonController.GetGachiSongs();
        CommandList = JsonController.GetCommandList();
    }
}