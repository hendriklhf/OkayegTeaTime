using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Configuration;
using OkayegTeaTime.Utils;
using SpotifyAPI.Web;

namespace OkayegTeaTime.Spotify;

public static class SpotifyController
{
    private static readonly List<ListeningSession> s_listeningSessions = [];

    private static List<string> ChatPlaylistUris
    {
        get
        {
            if (s_chatPlaylistUris is not null)
            {
                return s_chatPlaylistUris;
            }

#pragma warning disable S4462, VSTHRD002
            GetPlaylistTracksAsync().Wait();
#pragma warning restore VSTHRD002, S4462
            s_chatPlaylistUris ??= [];
            return s_chatPlaylistUris;

            // ReSharper disable once InconsistentNaming
            static async Task GetPlaylistTracksAsync()
            {
                string? username = DbController.GetUser(GlobalSettings.Settings.Users.Owner)?.Username;
                if (username is null)
                {
                    return;
                }

                Database.EntityFrameworkModels.Spotify? efUser = DbController.GetSpotifyUser(username);
                if (efUser is null)
                {
                    return;
                }

                SpotifyUser user = new(efUser);
                SpotifyTrack[] tracks = await GetPlaylistItemsAsync(user, GlobalSettings.Settings.OfflineChat!.ChatPlaylistId);
                s_chatPlaylistUris = tracks.Select(static t => t.Uri).ToList();
            }
        }
    }

#pragma warning disable IDE0032
    private static List<string>? s_chatPlaylistUris;
#pragma warning restore IDE0032

    /// <summary>
    /// The length of "spotify:track:".
    /// </summary>
    private const byte TrackIdPrefixLength = 14;

    public static string GetLoginUrl()
    {
        LoginRequest login = new(new("https://example.com/callback"), GlobalSettings.Settings.Spotify!.ClientId, LoginRequest.ResponseType.Code)
        {
            Scope = [Scopes.UserReadPlaybackState, Scopes.UserModifyPlaybackState]
        };

        return HttpUtility.UrlDecode(login.ToUri().AbsoluteUri).Replace(" ", "%20", StringComparison.Ordinal);
    }

    private static async ValueTask<string?> GetNewAccessTokenAsync(string refreshToken)
    {
        try
        {
            OAuthClient oAuthClient = new();
            AuthorizationCodeRefreshRequest authorizationCodeRefreshRequest = new(GlobalSettings.Settings.Spotify!.ClientId, GlobalSettings.Settings.Spotify!.ClientSecret, refreshToken);
            AuthorizationCodeRefreshResponse response = await oAuthClient.RequestToken(authorizationCodeRefreshRequest);
            return response.AccessToken;
        }
        catch (Exception ex)
        {
            await DbController.LogExceptionAsync(ex);
            return null;
        }
    }

    public static async ValueTask<(string AccessToken, string RefreshToken)?> GetNewAuthTokensAsync(string code)
    {
        try
        {
            OAuthClient oAuthClient = new();
            AuthorizationCodeTokenRequest authorizationCodeTokenRequest = new(GlobalSettings.Settings.Spotify!.ClientId, GlobalSettings.Settings.Spotify!.ClientSecret, code, new("https://example.com/callback"));
            AuthorizationCodeTokenResponse response = await oAuthClient.RequestToken(authorizationCodeTokenRequest);
            return (response.AccessToken, response.RefreshToken);
        }
        catch (Exception ex)
        {
            await DbController.LogExceptionAsync(ex);
            return null;
        }
    }

    public static string? ParseSongToUri(ReadOnlySpan<char> input)
    {
        if (Pattern.SpotifyUri.IsMatch(input))
        {
            return new(input);
        }

        if (Pattern.SpotifyLink.IsMatch(input))
        {
            string uriCode = Pattern.SpotifyTrackSlash.Match(new(input)).Value[6..];
            return $"spotify:track:{uriCode}";
        }

        return Pattern.SpotifyId.IsMatch(input) ? $"spotify:track:{new string(input)}" : null;
    }

    [SuppressMessage("Major Code Smell", "S6354:Use a testable date/time provider")]
    private static bool IsAccessTokenExpired(SpotifyUser user)
        => user.Time + TimeSpan.FromHours(1).TotalMilliseconds <= DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + TimeSpan.FromSeconds(30).TotalMilliseconds;

    private static async ValueTask<SpotifyClient?> GetClientAsync(SpotifyUser user)
    {
        if (!IsAccessTokenExpired(user))
        {
            return new(user.AccessToken);
        }

        string? accessToken = await GetNewAccessTokenAsync(user.RefreshToken);
        if (accessToken is null)
        {
            return null;
        }

        user.AccessToken = accessToken;
#pragma warning disable S6354
        user.Time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
#pragma warning restore S6354
        return new(user.AccessToken);
    }

    /// <summary>
    /// Adds the passed songs to the chat playlist in an own thread.
    /// </summary>
    /// <param name="user">The user privileged to add songs to the playlist.</param>
    /// <param name="songs">The songs that will be added to the playlist.</param>
    /// <exception cref="SpotifyException">Will be thrown if it was unable to add a song to the playlist.</exception>
    public static async ValueTask AddToPlaylistAsync(SpotifyUser user, params string[] songs)
    {
        string[] uris = songs.Select(static s => ParseSongToUri(s) ?? string.Empty).Where(static u => !string.IsNullOrWhiteSpace(u)).ToArray();
        if (uris.Length == 0)
        {
            return;
        }

        SpotifyClient client = await GetClientAsync(user) ?? throw new SpotifyException($"{user.Username.Antiping()} isn't registered, they have to register first");
        try
        {
            uris = uris.Where(static u => !ChatPlaylistUris.Contains(u)).ToArray();
            if (uris.Length == 0)
            {
                return;
            }

            await client.Playlists.AddItems(GlobalSettings.Settings.OfflineChat!.ChatPlaylistId, new(uris));
            ChatPlaylistUris.AddRange(uris);
        }
        catch (Exception ex)
        {
            await DbController.LogExceptionAsync(ex);
            throw new SpotifyException("Something went wrong trying to add the song to the playlist");
        }
    }

    private static async ValueTask<SpotifyTrack[]> GetPlaylistItemsAsync(SpotifyUser user, string playlistUri)
    {
        SpotifyClient client = await GetClientAsync(user) ?? throw new SpotifyException($"{user.Username.Antiping()} isn't registered, they have to register first");
        List<SpotifyTrack> result = new(2048);
        int offset = 0;
        do
        {
            try
            {
                Paging<PlaylistTrack<IPlayableItem>> playlistItems = await client.Playlists.GetItems(playlistUri, new()
                {
                    Offset = offset
                }, default);
                // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
                SpotifyTrack[]? items = playlistItems?.Items?.Select(static i => new SpotifyTrack(i.Track)).ToArray();
                if (items is null)
                {
                    break;
                }

                result.AddRange(items);
                if (items.Length < 100)
                {
                    break;
                }

                offset += 100;
            }
            catch (Exception ex)
            {
                await DbController.LogExceptionAsync(ex);
                break;
            }
        }
        while (true);

        return result.ToArray();
    }

    public static async ValueTask<SpotifyTrack> AddToQueueAsync(SpotifyUser user, string song)
    {
        if (!user.AreSongRequestsEnabled)
        {
            throw new SpotifyException($"song requests are currently not enabled, {user.Username.Antiping()} or a moderator has to enable it first");
        }

        string? uri = ParseSongToUri(song);
        if (uri is null)
        {
            SpotifyTrack? searchResult = await SearchTrackAsync(user, song);
            uri = searchResult?.Uri;
        }

        if (uri is null)
        {
            throw new SpotifyException("no matching track could be found");
        }

        try
        {
            SpotifyClient client = await GetClientAsync(user) ?? throw new SpotifyException($"{user.Username.Antiping()} isn't registered, they have to register first");
            await client.Player.AddToQueue(new(uri));
            FullTrack item = await client.Tracks.Get(uri[TrackIdPrefixLength..]);
            return new(item);
        }
        catch (APIException ex)
        {
            await DbController.LogExceptionAsync(ex);
            throw new SpotifyException($"an error occurred, {user.Username.Antiping()} probably has to start their playback first");
        }
        catch (Exception ex)
        {
            await DbController.LogExceptionAsync(ex);
            throw new SpotifyException($"an error occurred, it might not be possible to add songs to {user.Username.Antiping()}'s queue");
        }
    }

    public static async ValueTask SkipAsync(SpotifyUser user)
    {
        if (!user.AreSongRequestsEnabled)
        {
            throw new SpotifyException($"song requests are currently not enabled, {user.Username.Antiping()} or a moderator has to enable it first");
        }

        try
        {
            SpotifyClient client = await GetClientAsync(user) ?? throw new SpotifyException($"{user.Username.Antiping()} isn't registered, they have to register first");
            await client.Player.SkipNext();
        }
        catch (Exception ex)
        {
            await DbController.LogExceptionAsync(ex);
            throw new SpotifyException($"an error occurred, it might not be possible to skip songs of {user.Username.Antiping()}'s queue");
        }
    }

    public static ListeningSession? GetListeningSession(SpotifyUser host) => s_listeningSessions.Find(s => ReferenceEquals(s.Host, host));

    private static ListeningSession GetOrCreateListeningSession(SpotifyUser host)
    {
        ListeningSession? session = GetListeningSession(host);
        if (session is not null)
        {
            return session;
        }

        session = new(host);
        s_listeningSessions.Add(session);
        return session;
    }

    public static async ValueTask<SpotifyItem> ListenAlongWithAsync(SpotifyUser listener, SpotifyUser host)
    {
        if (string.Equals(host.Username, listener.Username, StringComparison.OrdinalIgnoreCase))
        {
            throw new SpotifyException("you can't listen to yourself :)");
        }

        CurrentlyPlayingContext playback = await GetCurrentlyPlayingContextAsync(host) ?? throw new SpotifyException($"{host.Username.Antiping()} isn't listening to anything at the moment");
        SpotifyItem item = playback.Item switch
        {
            FullTrack track => new SpotifyTrack(track),
            FullEpisode episode => new SpotifyEpisode(episode),
            _ => new(playback.Item)
        };

        int seekTo = playback.ProgressMs > 500 ? playback.ProgressMs : 0;
        await ListenToAsync(listener, item, seekTo);
        ListeningSession? listenerSession = GetListeningSession(listener);
        if (listenerSession is not null)
        {
            listenerSession.Listeners.Clear();
            listenerSession.StopTimer();
            s_listeningSessions.Remove(listenerSession);
            listenerSession.Dispose();
        }

        ListeningSession hostSession = GetOrCreateListeningSession(host);
        if (!hostSession.Listeners.Contains(listener))
        {
            hostSession.Listeners.Add(listener);
        }

        int interval = item.Duration - playback.ProgressMs;
        hostSession.StartTimer(interval);
        return item;
    }

    public static async ValueTask ListenToAsync(SpotifyUser user, SpotifyItem item, int seekToMs = 0)
    {
        try
        {
            SpotifyClient client = await GetClientAsync(user) ?? throw new SpotifyException($"{user.Username.Antiping()} isn't registered, they have to register first");
            await client.Player.AddToQueue(new(item.Uri));
        }
        catch (APIException ex)
        {
            await DbController.LogExceptionAsync(ex);
            throw new SpotifyException($"an error occurred, {user.Username.Antiping()} probably has to start their playback first");
        }
        catch (Exception ex)
        {
            await DbController.LogExceptionAsync(ex);
            throw new SpotifyException("an error occurred, it might not be possible to listen to other people's songs");
        }

        try
        {
            SpotifyClient client = await GetClientAsync(user) ?? throw new SpotifyException($"{user.Username.Antiping()} isn't registered, they have to register first");
            await client.Player.SkipNext();
            if (seekToMs != 0)
            {
                await client.Player.SeekTo(new(seekToMs));
            }
        }
        catch (Exception ex)
        {
            await DbController.LogExceptionAsync(ex);
            throw new SpotifyException($"an error occured while trying to play the song {user.Username.Antiping()} wanted to listen to");
        }
    }

    public static async ValueTask<SpotifyItem?> GetCurrentlyPlayingItemAsync(SpotifyUser user)
    {
        CurrentlyPlaying? currentlyPlaying;
        try
        {
            SpotifyClient client = await GetClientAsync(user) ?? throw new SpotifyException($"{user.Username.Antiping()} isn't registered, they have to register first");
            currentlyPlaying = await client.Player.GetCurrentlyPlaying(new());

            // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
            if (currentlyPlaying?.IsPlaying == false)
            {
                return null;
            }
        }
        catch (Exception ex)
        {
            await DbController.LogExceptionAsync(ex);
            throw new SpotifyException($"an error occurred, it might not be possible to retrieve {user.Username.Antiping()}'s currently playing song");
        }

        SpotifyItem? item = null;
        switch (currentlyPlaying?.Item)
        {
            case FullTrack track:
                item = new SpotifyTrack(track);

#if RELEASE
                if (GlobalSettings.Settings.OfflineChat is null)
                {
                    // not configured, return
                    return item;
                }

                if (!GlobalSettings.Settings.OfflineChat.PlaylistUsers.Contains(user.Id))
                {
                    return item;
                }

                if (item.IsLocal)
                {
                    return item;
                }

                string? username = DbController.GetUser(GlobalSettings.Settings.Users.Owner)?.Username;
                if (username is null)
                {
                    return item;
                }

                Database.EntityFrameworkModels.Spotify? efUser = DbController.GetSpotifyUser(username);
                if (efUser is null)
                {
                    return item;
                }

                SpotifyUser playlistUser = new(efUser);
                try
                {
                    await AddToPlaylistAsync(playlistUser, item.Uri);
                }
                catch (SpotifyException ex)
                {
                    await DbController.LogExceptionAsync(ex);
                }
#endif
                break;
            case FullEpisode episode:
                item = new SpotifyEpisode(episode);
                break;
        }

        return item;
    }

    public static async ValueTask<SpotifyTrack?> SearchTrackAsync(SpotifyUser user, string query)
    {
        SpotifyClient client = await GetClientAsync(user) ?? throw new SpotifyException($"{user.Username.Antiping()} isn't registered, they have to register first");
        string? uri = ParseSongToUri(query);
        if (uri is not null)
        {
            try
            {
                FullTrack? track = await client.Tracks.Get(uri.Split(':')[^1]);
                // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                if (track is not null)
                {
                    return new(track);
                }
            }
            catch (Exception ex)
            {
                await DbController.LogExceptionAsync(ex);
            }
        }

        try
        {
            SearchResponse searchResult = await client.Search.Item(new(SearchRequest.Types.Track, query));
            if (searchResult.Tracks.Items is null || searchResult.Tracks.Items.Count < 1)
            {
                return null;
            }

            FullTrack result = searchResult.Tracks.Items[0];
            return new(result);
        }
        catch (Exception ex)
        {
            await DbController.LogExceptionAsync(ex);
            return null;
        }
    }

    private static async ValueTask<CurrentlyPlayingContext?> GetCurrentlyPlayingContextAsync(SpotifyUser user)
    {
        SpotifyClient client = await GetClientAsync(user) ?? throw new SpotifyException($"{user.Username.Antiping()} isn't registered, they have to register first");
        CurrentlyPlayingContext? playback = await client.Player.GetCurrentPlayback();
        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        return playback?.IsPlaying != true ? null : playback;
    }

    public static SpotifyUser? GetListeningTo(SpotifyUser listener) => s_listeningSessions.Find(s => s.Listeners.Contains(listener))?.Host;

    public static async ValueTask<string[]> GetGenresAsync(SpotifyUser user, SpotifyTrack track)
    {
        SpotifyClient client = await GetClientAsync(user) ?? throw new SpotifyException($"{user.Username.Antiping()} isn't registered, they have to register first");
        if (track.Artists.Count == 1)
        {
            FullArtist artist = await client.Artists.Get(track.Artists[0].Id);
            return artist.Genres.ToArray();
        }

        List<string> artistIds = track.Artists.ConvertAll(static a => a.Id);
        ArtistsResponse artists = await client.Artists.GetSeveral(new(artistIds));
        return artists.Artists.Select(static a => a.Genres).SelectMany(static a => a).Distinct().ToArray();
    }
}
