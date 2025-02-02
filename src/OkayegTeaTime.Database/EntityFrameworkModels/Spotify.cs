﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace OkayegTeaTime.Database.EntityFrameworkModels;

public sealed class Spotify
{
    public long Id { get; set; }

    public string Username { get; set; }

    public string AccessToken { get; set; }

    public string RefreshToken { get; set; }

    [SuppressMessage("Major Code Smell", "S6354:Use a testable date/time provider")]
    public long Time { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    public bool SongRequestEnabled { get; set; }

    public Spotify(string username, string accessToken, string refreshToken)
    {
        Username = username;
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }

    public Spotify(long id, string username, string accessToken, string refreshToken, long time)
    {
        Id = id;
        Username = username;
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        Time = time;
    }
}
