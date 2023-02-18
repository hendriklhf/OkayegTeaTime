using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Files;
using OkayegTeaTime.Utils;
using SpotifyAPI.Web;

namespace OkayegTeaTime.Spotify;

public static class SpotifyController
{
    private static readonly List<ListeningSession> _listeningSessions = new();

    private static List<string> ChatPlaylistUris
    {
        get
        {
            if (_chatPlaylistUris is not null)
            {
                return _chatPlaylistUris;
            }

            static async Task GetPlaylistTracks()
            {
                string? username = DbController.GetUser(AppSettings.UserLists.Owner)?.Username;
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
                SpotifyTrack[] tracks = await GetPlaylistItemsAsync(user, AppSettings.Spotify.ChatPlaylistId);
                _chatPlaylistUris = tracks.Select(t => t.Uri).ToList();
            }

            GetPlaylistTracks().Wait();
            _chatPlaylistUris ??= new();
            return _chatPlaylistUris;
        }
    }

    private static List<string>? _chatPlaylistUris;

    /// <summary>
    ///     The length of "spotify:track:".
    /// </summary>
    private const byte _trackIdPrefixLength = 14;

    public static string GetLoginUrl()
    {
        LoginRequest login = new(new("https://example.com/callback"), AppSettings.Spotify.ClientId, LoginRequest.ResponseType.Code)
        {
            Scope = new[]
            {
                Scopes.UserReadPlaybackState,
                Scopes.UserModifyPlaybackState
            }
        };

        return HttpUtility.UrlDecode(login.ToUri().AbsoluteUri).Replace(" ", "%20");
    }

    private static async Task<string?> GetNewAccessTokenAsync(string refreshToken)
    {
        try
        {
            AuthorizationCodeRefreshResponse response = await new OAuthClient().RequestToken(new AuthorizationCodeRefreshRequest(AppSettings.Spotify.ClientId, AppSettings.Spotify.ClientSecret, refreshToken));
            return response.AccessToken;
        }
        catch (Exception ex)
        {
            DbController.LogException(ex);
            return null;
        }
    }

    public static async Task<(string AccessToken, string RefreshToken)?> GetNewAuthTokensAsync(string code)
    {
        try
        {
            AuthorizationCodeTokenResponse response = await new OAuthClient().RequestToken(new AuthorizationCodeTokenRequest(AppSettings.Spotify.ClientId, AppSettings.Spotify.ClientSecret, code, new("https://example.com/callback")));
            return (response.AccessToken, response.RefreshToken);
        }
        catch (Exception ex)
        {
            DbController.LogException(ex);
            return null;
        }
    }

    public static string? ParseSongToUri(string input)
    {
        if (Pattern.SpotifyUri.IsMatch(input))
        {
            return input;
        }

        if (Pattern.SpotifyLink.IsMatch(input))
        {
            string uriCode = Regex.Match(input, @"track/\w+").Value[6..];
            return $"spotify:track:{uriCode}";
        }

        return Regex.IsMatch(input, @"^\w{22}$") ? $"spotify:track:{input}" : null;
    }

    private static bool IsAccessTokenExpired(SpotifyUser user)
    {
        return user.Time + TimeSpan.FromHours(1).TotalMilliseconds <= DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + TimeSpan.FromSeconds(30).TotalMilliseconds;
    }

    private static async Task<SpotifyClient?> GetClient(SpotifyUser user)
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
        user.Time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        return new(user.AccessToken);
    }

    /// <summary>
    /// Adds the passed songs to the chat playlist in an own thread.
    /// </summary>
    /// <param name="user">The user privileged to add songs to the playlist.</param>
    /// <param name="songs">The songs that will be added to the playlist.</param>
    /// <exception cref="SpotifyException">Will be thrown if it was unable to add a song to the playlist.</exception>
    public static void AddToPlaylist(SpotifyUser user, params string[] songs)
    {
        async Task AddToChatPlaylistLocal()
        {
            string[] uris = songs.Select(s => ParseSongToUri(s) ?? string.Empty).Where(u => !string.IsNullOrWhiteSpace(u)).ToArray();
            if (uris.Length == 0)
            {
                return;
            }

            SpotifyClient? client = await GetClient(user);
            if (client is null)
            {
                throw new SpotifyException($"{user.Username.Antiping()} isn't registered, they have to register first");
            }

            try
            {
                uris = uris.Where(u => !ChatPlaylistUris.Contains(u)).ToArray();
                if (uris.Length == 0)
                {
                    return;
                }

                await client.Playlists.AddItems(AppSettings.Spotify.ChatPlaylistId, new(uris));
                ChatPlaylistUris.AddRange(uris);
            }
            catch (Exception ex)
            {
                DbController.LogException(ex);
                throw new SpotifyException("Something went wrong trying to add the song to the playlist");
            }
        }

        Thread thread = new(() => AddToChatPlaylistLocal().Wait());
        thread.Start();
    }

    private static async Task<SpotifyTrack[]> GetPlaylistItemsAsync(SpotifyUser user, string playlistUri)
    {
        SpotifyClient? client = await GetClient(user);
        if (client is null)
        {
            throw new SpotifyException($"{user.Username.Antiping()} isn't registered, they have to register first");
        }

        List<SpotifyTrack> result = new();
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
                SpotifyTrack[]? items = playlistItems?.Items?.Select(i => new SpotifyTrack(i.Track)).ToArray();
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
                DbController.LogException(ex);
                break;
            }
        } while (true);

        return result.ToArray();
    }

    public static async Task<SpotifyTrack> AddToQueueAsync(SpotifyUser user, string song)
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
            SpotifyClient? client = await GetClient(user);
            if (client is null)
            {
                throw new SpotifyException($"{user.Username.Antiping()} isn't registered, they have to register first");
            }

            await client.Player.AddToQueue(new(uri));
            FullTrack item = await client.Tracks.Get(uri[_trackIdPrefixLength..]);
            return new(item);
        }
        catch (APIException ex)
        {
            DbController.LogException(ex);
            throw new SpotifyException($"an error occurred, {user.Username.Antiping()} probably has to start their playback first");
        }
        catch (Exception ex)
        {
            DbController.LogException(ex);
            throw new SpotifyException($"an error occurred, it might not be possible to add songs to {user.Username.Antiping()}'s queue");
        }
    }

    public static async Task SkipAsync(SpotifyUser user)
    {
        if (!user.AreSongRequestsEnabled)
        {
            throw new SpotifyException($"song requests are currently not enabled, {user.Username.Antiping()} or a moderator has to enable it first");
        }

        try
        {
            SpotifyClient? client = await GetClient(user);
            if (client is null)
            {
                throw new SpotifyException($"{user.Username.Antiping()} isn't registered, they have to register first");
            }

            await client.Player.SkipNext();
        }
        catch (Exception ex)
        {
            DbController.LogException(ex);
            throw new SpotifyException($"an error occurred, it might not be possible to skip songs of {user.Username.Antiping()}'s queue");
        }
    }

    public static ListeningSession? GetListeningSession(SpotifyUser host)
    {
        return _listeningSessions.FirstOrDefault(s => ReferenceEquals(s.Host, host));
    }

    private static ListeningSession GetOrCreateListeningSession(SpotifyUser host)
    {
        ListeningSession? session = GetListeningSession(host);
        if (session is not null)
        {
            return session;
        }

        session = new(host);
        _listeningSessions.Add(session);
        return session;
    }

    public static async Task<SpotifyItem> ListenAlongWithAsync(SpotifyUser listener, SpotifyUser host)
    {
        if (string.Equals(host.Username, listener.Username, StringComparison.OrdinalIgnoreCase))
        {
            throw new SpotifyException("you can't listen to yourself :)");
        }

        CurrentlyPlayingContext? playback = await GetCurrentlyPlayingContextAsync(host);
        if (playback is null)
        {
            throw new SpotifyException($"{host.Username.Antiping()} isn't listening to anything at the moment");
        }

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
            _listeningSessions.Remove(listenerSession);
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

    public static async Task ListenToAsync(SpotifyUser user, SpotifyItem item, int seekToMs = 0)
    {
        try
        {
            SpotifyClient? client = await GetClient(user);
            if (client is null)
            {
                throw new SpotifyException($"{user.Username.Antiping()} isn't registered, they have to register first");
            }

            await client.Player.AddToQueue(new(item.Uri));
        }
        catch (APIException ex)
        {
            DbController.LogException(ex);
            throw new SpotifyException($"an error occurred, {user.Username.Antiping()} probably has to start their playback first");
        }
        catch (Exception ex)
        {
            DbController.LogException(ex);
            throw new SpotifyException("an error occurred, it might not be possible to listen to other people's songs");
        }

        try
        {
            SpotifyClient? client = await GetClient(user);
            if (client is null)
            {
                throw new SpotifyException($"{user.Username.Antiping()} isn't registered, they have to register first");
            }

            await client.Player.SkipNext();
            if (seekToMs > 0)
            {
                await client.Player.SeekTo(new(seekToMs));
            }
        }
        catch (Exception ex)
        {
            DbController.LogException(ex);
            throw new SpotifyException($"an error occured while trying to play the song {user.Username.Antiping()} wanted to listen to");
        }
    }

    public static async Task<SpotifyItem?> GetCurrentlyPlayingItemAsync(SpotifyUser user)
    {
        CurrentlyPlaying? currentlyPlaying;
        try
        {
            SpotifyClient? client = await GetClient(user);
            if (client is null)
            {
                throw new SpotifyException($"{user.Username.Antiping()} isn't registered, they have to register first");
            }

            currentlyPlaying = await client.Player.GetCurrentlyPlaying(new());

            // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
            if (currentlyPlaying?.IsPlaying == false)
            {
                return null;
            }
        }
        catch (Exception ex)
        {
            DbController.LogException(ex);
            throw new SpotifyException($"an error occurred, it might not be possible to retrieve {user.Username.Antiping()}'s currently playing song");
        }

        SpotifyItem? item = null;
        switch (currentlyPlaying?.Item)
        {
            case FullTrack track:
            {
                item = new SpotifyTrack(track);

#if RELEASE
                if (!AppSettings.Spotify.ChatPlaylistUsers.Contains(user.Id))
                {
                    return item;
                }

                if (item.IsLocal)
                {
                    return item;
                }

                string? username = DbController.GetUser(AppSettings.UserLists.Owner)?.Username;
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
                    AddToPlaylist(playlistUser, item.Uri);
                }
                catch (SpotifyException ex)
                {
                    DbController.LogException(ex);
                }
#endif
                break;
            }
            case FullEpisode episode:
            {
                item = new SpotifyEpisode(episode);
                break;
            }
        }

        return item;
    }

    public static async Task<SpotifyTrack?> SearchTrackAsync(SpotifyUser user, string query)
    {
        SpotifyClient? client = await GetClient(user);
        if (client is null)
        {
            throw new SpotifyException($"{user.Username.Antiping()} isn't registered, they have to register first");
        }

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
                DbController.LogException(ex);
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
            DbController.LogException(ex);
            return null;
        }
    }

    private static async Task<CurrentlyPlayingContext?> GetCurrentlyPlayingContextAsync(SpotifyUser user)
    {
        SpotifyClient? client = await GetClient(user);
        if (client is null)
        {
            throw new SpotifyException($"{user.Username.Antiping()} isn't registered, they have to register first");
        }

        CurrentlyPlayingContext? playback = await client.Player.GetCurrentPlayback();
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (playback is null || !playback.IsPlaying)
        {
            return null;
        }

        return playback;
    }

    public static SpotifyUser? GetListeningTo(SpotifyUser listener)
    {
        return _listeningSessions.FirstOrDefault(s => s.Listeners.Contains(listener))?.Host;
    }

    public static async Task<string[]> GetGenresAsync(SpotifyUser user, SpotifyTrack track)
    {
        SpotifyClient? client = await GetClient(user);
        if (client is null)
        {
            throw new SpotifyException($"{user.Username.Antiping()} isn't registered, they have to register first");
        }

        if (track.Artists.Count == 1)
        {
            FullArtist artist = await client.Artists.Get(track.Artists[0].Id);
            return artist.Genres.ToArray();
        }

        List<string> artistIds = track.Artists.Select(a => a.Id).ToList();
        ArtistsResponse artists = await client.Artists.GetSeveral(new(artistIds));
        return artists.Artists.Select(a => a.Genres).SelectMany(a => a).Distinct().ToArray();
    }
}
