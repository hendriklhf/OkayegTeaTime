using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE.Strings;
using HLE.Twitch.Models;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Configuration;
using OkayegTeaTime.Spotify;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Listen, typeof(ListenCommand))]
public readonly struct ListenCommand(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    : IChatCommand<ListenCommand>
{
    public PooledStringBuilder Response { get; } = new(GlobalSettings.MaxMessageLength);

    public IChatMessage ChatMessage { get; } = chatMessage;

    private readonly TwitchBot _twitchBot = twitchBot;
    private readonly ReadOnlyMemory<char> _prefix = prefix;
    private readonly ReadOnlyMemory<char> _alias = alias;

    private const string LocalFileDescription = "local file";

    public static void Create(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out ListenCommand command)
        => command = new(twitchBot, chatMessage, prefix, alias);

    public async ValueTask HandleAsync()
    {
        if (GlobalSettings.Settings.OfflineChat is null || GlobalSettings.Settings.Spotify is null)
        {
            Response.Append(ChatMessage.Username, ", ", Texts.TheCommandHasNotBeenConfiguredByTheBotOwner);
            return;
        }

        if (!GlobalSettings.Settings.OfflineChat.Users.Contains(ChatMessage.UserId))
        {
            Response.Append(ChatMessage.Username, ", this command is still being tested, you aren't allowed to use this command");
            return;
        }

        Regex pattern = _twitchBot.MessageRegexCreator.Create(_alias.Span, _prefix.Span, @"\s((leave)|(stop))");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            StopListening();
            return;
        }

        pattern = _twitchBot.MessageRegexCreator.Create(_alias.Span, _prefix.Span, @"\ssync");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            await SyncListeningAsync();
            return;
        }

        pattern = _twitchBot.MessageRegexCreator.Create(_alias.Span, _prefix.Span, @"\s\w+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            await ListenToUserAsync();
        }
    }

    private async ValueTask ListenToUserAsync()
    {
        SpotifyUser? listener = _twitchBot.SpotifyUsers[ChatMessage.Username];
        if (listener is null)
        {
            Response.Append(ChatMessage.Username, ", ", Texts.YouCantListenToOtherUsersYouHaveToRegisterFirst);
            return;
        }

        using ChatMessageExtension messageExtension = new(ChatMessage);
        string hostUsername = StringPool.Shared.GetOrAdd(messageExtension.LowerSplit[1].Span);
        SpotifyUser? host = _twitchBot.SpotifyUsers[hostUsername];
        if (host is null)
        {
            Response.Append(ChatMessage.Username, ", you can't listen to ", hostUsername, "'s music , they have to register first");
            return;
        }

        SpotifyItem item;
        try
        {
            item = await SpotifyController.ListenAlongWithAsync(listener, host);
        }
        catch (SpotifyException ex)
        {
            Response.Append(ChatMessage.Username, ", ", ex.Message);
            return;
        }
        catch (AggregateException ex)
        {
            Response.Append(ChatMessage.Username, ", ");
            if (ex.InnerException is null)
            {
                await DbController.LogExceptionAsync(ex);
                Response.Append(Texts.ApiError);
                return;
            }

            Response.Append(ex.InnerException.Message);
            return;
        }

        Response.Append(ChatMessage.Username, ", ", "now listening along with ", host.Username.Antiping(), " and playing ");
        switch (item)
        {
            case SpotifyTrack track:
                Response.Append(track.Name, " by ");

                string[] artists = track.Artists.Select(static a => a.Name).ToArray();
                int joinLength = StringHelpers.Join(", ", artists, Response.FreeBufferSpan);
                Response.Advance(joinLength);

                Response.Append(" || ", track.IsLocal ? LocalFileDescription : track.Uri);
                break;
            case SpotifyEpisode episode:
                Response.Append(episode.Name, " by ", episode.Show.Name, " || ", episode.IsLocal ? LocalFileDescription : episode.Uri);
                break;
            default:
                Response.Append("an unknown item type monkaS");
                break;
        }
    }

    private async ValueTask SyncListeningAsync()
    {
        SpotifyUser? listener = _twitchBot.SpotifyUsers[ChatMessage.Username];
        if (listener is null)
        {
            Response.Append(ChatMessage.Username, ", ", Texts.YouCantSyncYouHaveToRegisterFirst);
            return;
        }

        SpotifyUser? host = SpotifyController.GetListeningTo(listener);
        if (host is null)
        {
            Response.Append(ChatMessage.Username, ", ", Texts.YouCantSyncBecauseYouArentListeningAlongWithAnybody);
            return;
        }

        SpotifyItem item;
        try
        {
            item = await SpotifyController.ListenAlongWithAsync(listener, host);
        }
        catch (SpotifyException ex)
        {
            Response.Append(ChatMessage.Username, ", ", ex.Message);
            return;
        }
        catch (AggregateException ex)
        {
            Response.Append(ChatMessage.Username, ", ");
            if (ex.InnerException is null)
            {
                await DbController.LogExceptionAsync(ex);
                Response.Append(Texts.ApiError);
                return;
            }

            Response.Append(ex.InnerException.Message);
            return;
        }

        Response.Append(ChatMessage.Username, ", ", "synced with ", host.Username.Antiping(), " and playing ");
        switch (item)
        {
            case SpotifyTrack track:
                Response.Append(track.Name, " by ");

                string[] artists = track.Artists.Select(static a => a.Name).ToArray();
                int joinLength = StringHelpers.Join(", ", artists, Response.FreeBufferSpan);
                Response.Advance(joinLength);

                Response.Append(" || ", track.IsLocal ? LocalFileDescription : track.Uri);
                break;
            case SpotifyEpisode episode:
                Response.Append(episode.Name, " by ", episode.Show.Name, " || ", episode.IsLocal ? LocalFileDescription : episode.Uri);
                break;
            default:
                Response.Append("an unknown item type monkaS");
                break;
        }
    }

    private void StopListening()
    {
        SpotifyUser? listener = _twitchBot.SpotifyUsers[ChatMessage.Username];
        if (listener is null)
        {
            Response.Append(ChatMessage.Username, ", ", Texts.YouArentRegisteredYouHaveToRegisterFirst);
            return;
        }

        SpotifyUser? host = SpotifyController.GetListeningTo(listener);
        if (host is null)
        {
            Response.Append(ChatMessage.Username, ", ", Texts.YouArentListeningAlongWithAnybody);
            return;
        }

        ListeningSession? listeningSession = SpotifyController.GetListeningSession(host);
        listeningSession?.Listeners.Remove(listener);
        Response.Append(ChatMessage.Username, ", ", "stopped listening along with ", host.Username.Antiping());
    }

    public void Dispose() => Response.Dispose();

    public bool Equals(ListenCommand other) =>
        _twitchBot.Equals(other._twitchBot) && _prefix.Equals(other._prefix) && _alias.Equals(other._alias) &&
        Response.Equals(other.Response) && ChatMessage.Equals(other.ChatMessage);

    public override bool Equals(object? obj) => obj is ListenCommand other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(_twitchBot, _prefix, _alias, Response, ChatMessage);

    public static bool operator ==(ListenCommand left, ListenCommand right) => left.Equals(right);

    public static bool operator !=(ListenCommand left, ListenCommand right) => !left.Equals(right);
}
