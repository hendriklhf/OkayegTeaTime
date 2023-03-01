using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE.Twitch;
using HLE.Twitch.Models;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Spotify;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;
using StringHelper = HLE.StringHelper;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Listen)]
public readonly ref struct ListenCommand
{
    public ChatMessage ChatMessage { get; }

    private readonly ref MessageBuilder _response;

    private readonly TwitchBot _twitchBot;
    private readonly string? _prefix;
    private readonly string _alias;

    public ListenCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, string? prefix, string alias)
    {
        ChatMessage = chatMessage;
        _response = ref response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        if (!AppSettings.UserLists.SecretUsers.Contains(ChatMessage.UserId))
        {
            _response.Append(ChatMessage.Username, ", ", "this command is still being tested, you aren't allowed to use this command");
            return;
        }

        Regex pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\s((leave)|(stop))");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            StopListening();
            return;
        }

        pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\ssync");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            SyncListening();
            return;
        }

        pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\s\w+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            ListenToUser();
        }
    }

    private void ListenToUser()
    {
        SpotifyUser? listener = _twitchBot.SpotifyUsers[ChatMessage.Username];
        if (listener is null)
        {
            _response.Append(ChatMessage.Username, ", ", Messages.YouCantListenToOtherUsersYouHaveToRegisterFirst);
            return;
        }

        using ChatMessageExtension messageExtension = new(ChatMessage);
        string hostUsername = new(messageExtension.LowerSplit[1]);
        SpotifyUser? host = _twitchBot.SpotifyUsers[hostUsername];
        if (host is null)
        {
            _response.Append(ChatMessage.Username, ", ", "you can't listen to ", hostUsername, "'s music , they have to register first");
            return;
        }

        SpotifyItem item;
        try
        {
            Task<SpotifyItem> task = SpotifyController.ListenAlongWithAsync(listener, host);
            task.Wait();
            item = task.Result;
        }
        catch (SpotifyException ex)
        {
            _response.Append(ChatMessage.Username, ", ", ex.Message);
            return;
        }
        catch (AggregateException ex)
        {
            _response.Append(ChatMessage.Username, ", ");
            if (ex.InnerException is null)
            {
                DbController.LogException(ex);
                _response.Append(Messages.ApiError);
                return;
            }

            _response.Append(ex.InnerException.Message);
            return;
        }

        _response.Append(ChatMessage.Username, ", ", "now listening along with ", host.Username.Antiping(), " and playing ");
        switch (item)
        {
            case SpotifyTrack track:
            {
                string[] artists = track.Artists.Select(a => a.Name).ToArray();
                Span<char> joinBuffer = stackalloc char[250];
                int bufferLength = StringHelper.Join(artists, ", ", joinBuffer);
                _response.Append(track.Name, " by ", joinBuffer[..bufferLength], " || ", track.IsLocal ? "local file" : track.Uri);
                break;
            }
            case SpotifyEpisode episode:
            {
                _response.Append(episode.Name, " by ", episode.Show.Name, " || ", episode.IsLocal ? "local file" : episode.Uri);
                break;
            }
            default:
            {
                _response.Append("an unknown item type monkaS");
                break;
            }
        }
    }

    private void SyncListening()
    {
        SpotifyUser? listener = _twitchBot.SpotifyUsers[ChatMessage.Username];
        if (listener is null)
        {
            _response.Append(ChatMessage.Username, ", ", Messages.YouCantSyncYouHaveToRegisterFirst);
            return;
        }

        SpotifyUser? host = SpotifyController.GetListeningTo(listener);
        if (host is null)
        {
            _response.Append(ChatMessage.Username, ", ", Messages.YouCantSyncBecauseYouArentListeningAlongWithAnybody);
            return;
        }

        SpotifyItem item;
        try
        {
            Task<SpotifyItem> task = SpotifyController.ListenAlongWithAsync(listener, host);
            task.Wait();
            item = task.Result;
        }
        catch (SpotifyException ex)
        {
            _response.Append(ChatMessage.Username, ", ", ex.Message);
            return;
        }
        catch (AggregateException ex)
        {
            _response.Append(ChatMessage.Username, ", ");
            if (ex.InnerException is null)
            {
                DbController.LogException(ex);
                _response.Append(Messages.ApiError);
                return;
            }

            _response.Append(ex.InnerException.Message);
            return;
        }

        _response.Append(ChatMessage.Username, ", ", "synced with ", host.Username.Antiping(), " and playing ");
        switch (item)
        {
            case SpotifyTrack track:
                string[] artists = track.Artists.Select(a => a.Name).ToArray();
                Span<char> joinBuffer = stackalloc char[250];
                int bufferLength = StringHelper.Join(artists, ", ", joinBuffer);
                _response.Append(track.Name, " by ", joinBuffer[..bufferLength], " || ", track.IsLocal ? "local file" : track.Uri);
                break;
            case SpotifyEpisode episode:
                _response.Append(episode.Name, " by ", episode.Show.Name, " || ", episode.IsLocal ? "local file" : episode.Uri);
                break;
            default:
                _response.Append("an unknown item type monkaS");
                break;
        }
    }

    private void StopListening()
    {
        SpotifyUser? listener = _twitchBot.SpotifyUsers[ChatMessage.Username];
        if (listener is null)
        {
            _response.Append(ChatMessage.Username, ", ", Messages.YouArentRegisteredYouHaveToRegisterFirst);
            return;
        }

        SpotifyUser? host = SpotifyController.GetListeningTo(listener);
        if (host is null)
        {
            _response.Append(ChatMessage.Username, ", ", Messages.YouArentListeningAlongWithAnybody);
            return;
        }

        ListeningSession? listeningSession = SpotifyController.GetListeningSession(host);
        listeningSession?.Listeners.Remove(listener);
        _response.Append(ChatMessage.Username, ", ", "stopped listening along with ", host.Username.Antiping());
    }
}
