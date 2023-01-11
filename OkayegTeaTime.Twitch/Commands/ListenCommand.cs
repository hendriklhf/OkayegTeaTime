using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Files;
using OkayegTeaTime.Spotify;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Listen)]
public readonly unsafe ref struct ListenCommand
{
    public TwitchChatMessage ChatMessage { get; }

    public StringBuilder* Response { get; }

    private readonly TwitchBot _twitchBot;
    private readonly string? _prefix;
    private readonly string _alias;

    public ListenCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        ChatMessage = chatMessage;
        Response = response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        if (!AppSettings.UserLists.SecretUsers.Contains(ChatMessage.UserId))
        {
            Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, "this command is still being tested, you aren't allowed to use this command");
            return;
        }

        Regex pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\s((leave)|(stop))");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            SpotifyUser? listener = _twitchBot.SpotifyUsers[ChatMessage.Username];
            if (listener is null)
            {
                Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, PredefinedMessages.YouArentRegisteredYouHaveToRegisterFirst);
                return;
            }

            SpotifyUser? host = SpotifyController.GetListeningTo(listener);
            if (host is null)
            {
                Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, PredefinedMessages.YouArentListeningAlongWithAnybody);
                return;
            }

            ListeningSession? listeningSession = SpotifyController.GetListeningSession(host);
            listeningSession?.Listeners.Remove(listener);
            Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, "stopped listening along with ", host.Username.Antiping());
            return;
        }

        pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\ssync");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            SpotifyUser? listener = _twitchBot.SpotifyUsers[ChatMessage.Username];
            if (listener is null)
            {
                Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, PredefinedMessages.YouCantSyncYouHaveToRegisterFirst);
                return;
            }

            SpotifyUser? host = SpotifyController.GetListeningTo(listener);
            if (host is null)
            {
                Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, PredefinedMessages.YouCantSyncBecauseYouArentListeningAlongWithAnybody);
                return;
            }

            SpotifyItem item;
            try
            {
                Task<SpotifyItem> task = SpotifyController.ListenAlongWith(listener, host);
                task.Wait();
                item = task.Result;
            }
            catch (SpotifyException ex)
            {
                Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, ex.Message);
                return;
            }
            catch (AggregateException ex)
            {
                Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace);
                if (ex.InnerException is null)
                {
                    DbController.LogException(ex);
                    Response->Append(PredefinedMessages.ApiError);
                    return;
                }

                Response->Append(ex.InnerException.Message);
                return;
            }

            Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, "synced with ", host.Username.Antiping(), " and playing ");
            switch (item)
            {
                case SpotifyTrack track:
                    string artists = string.Join(PredefinedMessages.CommaSpace, track.Artists.Select(a => a.Name));
                    Response->Append(track.Name, " by ", artists, " || ", track.IsLocal ? "local file" : track.Uri);
                    break;
                case SpotifyEpisode episode:
                    Response->Append(episode.Name, " by ", episode.Show.Name, " || ", episode.IsLocal ? "local file" : episode.Uri);
                    break;
                default:
                    Response->Append("an unknown item type monkaS");
                    break;
            }

            return;
        }

        pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\s\w+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            SpotifyUser? listener = _twitchBot.SpotifyUsers[ChatMessage.Username];
            if (listener is null)
            {
                Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, PredefinedMessages.YouCantListenToOtherUsersYouHaveToRegisterFirst);
                return;
            }

            SpotifyUser? host = _twitchBot.SpotifyUsers[ChatMessage.LowerSplit[1]];
            if (host is null)
            {
                Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, "you can't listen to ", ChatMessage.LowerSplit[1], "'s music , they have to register first");
                return;
            }

            SpotifyItem item;
            try
            {
                Task<SpotifyItem> task = SpotifyController.ListenAlongWith(listener, host);
                task.Wait();
                item = task.Result;
            }
            catch (SpotifyException ex)
            {
                Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, ex.Message);
                return;
            }
            catch (AggregateException ex)
            {
                Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace);
                if (ex.InnerException is null)
                {
                    DbController.LogException(ex);
                    Response->Append(PredefinedMessages.ApiError);
                    return;
                }

                Response->Append(ex.InnerException.Message);
                return;
            }

            Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, "now listening along with ", host.Username.Antiping(), " and playing ");
            switch (item)
            {
                case SpotifyTrack track:
                {
                    string artists = string.Join(PredefinedMessages.CommaSpace, track.Artists.Select(a => a.Name));
                    Response->Append(track.Name, " by ", artists, " || ", track.IsLocal ? "local file" : track.Uri);
                    break;
                }
                case SpotifyEpisode episode:
                {
                    Response->Append(episode.Name, " by ", episode.Show.Name, " || ", episode.IsLocal ? "local file" : episode.Uri);
                    break;
                }
                default:
                {
                    Response->Append("an unknown item type monkaS");
                    break;
                }
            }
        }
    }
}
