using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE.Strings;
using HLE.Twitch.Models;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Spotify;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;
using StringHelper = HLE.Strings.StringHelper;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Listen, typeof(ListenCommand))]
public readonly struct ListenCommand : IChatCommand<ListenCommand>
{
    public ResponseBuilder Response { get; }

    public ChatMessage ChatMessage { get; }

    private readonly TwitchBot _twitchBot;
    private readonly ReadOnlyMemory<char> _prefix;
    private readonly ReadOnlyMemory<char> _alias;

    public ListenCommand(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    {
        ChatMessage = chatMessage;
        Response = new(AppSettings.MaxMessageLength);
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public static void Create(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out ListenCommand command)
    {
        command = new(twitchBot, chatMessage, prefix, alias);
    }

    public async ValueTask Handle()
    {
        if (!AppSettings.UserLists.SecretUsers.Contains(ChatMessage.UserId))
        {
            Response.Append(ChatMessage.Username, ", ", "this command is still being tested, you aren't allowed to use this command");
            return;
        }

        Regex pattern = _twitchBot.RegexCreator.Create(_alias.Span, _prefix.Span, @"\s((leave)|(stop))");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            StopListening();
            return;
        }

        pattern = _twitchBot.RegexCreator.Create(_alias.Span, _prefix.Span, @"\ssync");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            await SyncListening();
            return;
        }

        pattern = _twitchBot.RegexCreator.Create(_alias.Span, _prefix.Span, @"\s\w+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            await ListenToUser();
        }
    }

    private async ValueTask ListenToUser()
    {
        SpotifyUser? listener = _twitchBot.SpotifyUsers[ChatMessage.Username];
        if (listener is null)
        {
            Response.Append(ChatMessage.Username, ", ", Messages.YouCantListenToOtherUsersYouHaveToRegisterFirst);
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
                Response.Append(Messages.ApiError);
                return;
            }

            Response.Append(ex.InnerException.Message);
            return;
        }

        Response.Append(ChatMessage.Username, ", ", "now listening along with ", host.Username.Antiping(), " and playing ");
        switch (item)
        {
            case SpotifyTrack track:
            {
                Response.Append(track.Name, " by ");

                string[] artists = track.Artists.Select(a => a.Name).ToArray();
                int joinLength = StringHelper.Join(artists, ", ", Response.FreeBufferSpan);
                Response.Advance(joinLength);

                Response.Append(" || ", track.IsLocal ? "local file" : track.Uri);
                break;
            }
            case SpotifyEpisode episode:
            {
                Response.Append(episode.Name, " by ", episode.Show.Name, " || ", episode.IsLocal ? "local file" : episode.Uri);
                break;
            }
            default:
            {
                Response.Append("an unknown item type monkaS");
                break;
            }
        }
    }

    private async ValueTask SyncListening()
    {
        SpotifyUser? listener = _twitchBot.SpotifyUsers[ChatMessage.Username];
        if (listener is null)
        {
            Response.Append(ChatMessage.Username, ", ", Messages.YouCantSyncYouHaveToRegisterFirst);
            return;
        }

        SpotifyUser? host = SpotifyController.GetListeningTo(listener);
        if (host is null)
        {
            Response.Append(ChatMessage.Username, ", ", Messages.YouCantSyncBecauseYouArentListeningAlongWithAnybody);
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
                Response.Append(Messages.ApiError);
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

                string[] artists = track.Artists.Select(a => a.Name).ToArray();
                int joinLength = StringHelper.Join(artists, ", ", Response.FreeBufferSpan);
                Response.Advance(joinLength);

                Response.Append(" || ", track.IsLocal ? "local file" : track.Uri);
                break;
            case SpotifyEpisode episode:
                Response.Append(episode.Name, " by ", episode.Show.Name, " || ", episode.IsLocal ? "local file" : episode.Uri);
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
            Response.Append(ChatMessage.Username, ", ", Messages.YouArentRegisteredYouHaveToRegisterFirst);
            return;
        }

        SpotifyUser? host = SpotifyController.GetListeningTo(listener);
        if (host is null)
        {
            Response.Append(ChatMessage.Username, ", ", Messages.YouArentListeningAlongWithAnybody);
            return;
        }

        ListeningSession? listeningSession = SpotifyController.GetListeningSession(host);
        listeningSession?.Listeners.Remove(listener);
        Response.Append(ChatMessage.Username, ", ", "stopped listening along with ", host.Username.Antiping());
    }

    public void Dispose()
    {
        Response.Dispose();
    }
}
