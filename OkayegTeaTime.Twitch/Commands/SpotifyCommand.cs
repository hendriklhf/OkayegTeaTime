using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Spotify;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Spotify)]
public readonly unsafe ref struct SpotifyCommand
{
    public TwitchChatMessage ChatMessage { get; }

    public Response* Response { get; }

    private readonly TwitchBot _twitchBot;
    [SuppressMessage("ReSharper", "NotAccessedField.Local")]
    [SuppressMessage("CodeQuality", "IDE0052:Remove unread private members")]
    private readonly string? _prefix;
    [SuppressMessage("ReSharper", "NotAccessedField.Local")]
    [SuppressMessage("CodeQuality", "IDE0052:Remove unread private members")]
    private readonly string _alias;

    private static readonly Regex _urlPattern = new(@"^(-l)|(--url)$", RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

    public SpotifyCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, Response* response, string? prefix, string alias)
    {
        ChatMessage = chatMessage;
        Response = response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        string username = ChatMessage.LowerSplit.Length > 1 ? ChatMessage.LowerSplit[1] == "me" ? ChatMessage.Username : ChatMessage.LowerSplit[1] : ChatMessage.Channel;
        bool returnUrl = ChatMessage.LowerSplit.Any(s => _urlPattern.IsMatch(s));
        bool targetIsSender = username == ChatMessage.Username;
        SpotifyUser? user = _twitchBot.SpotifyUsers[username];
        if (user is null)
        {
            Response->Append(targetIsSender ? $"{ChatMessage.Username}, can't get your currently playing song, you have to register first"
                : $"{ChatMessage.Username}, can't get the currently playing song of {username.Antiping()}, they have to register first");
            return;
        }

        SpotifyItem? item;
        try
        {
            Task<SpotifyItem?> task = SpotifyController.GetCurrentlyPlayingItem(user);
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

        Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace);
        switch (item)
        {
            case null:
            {
                Response->Append(targetIsSender ? "you aren't listening to anything" : $"{username.Antiping()} is not listening to anything");
                return;
            }
            case SpotifyTrack track:
            {
                string artists = string.Join(", ", track.Artists.Select(a => a.Name));
                Response->Append(track.Name, " by ", artists, " || ", track.IsLocal ? "local file" : returnUrl ? track.Url : track.Uri);
                return;
            }
            case SpotifyEpisode episode:
            {
                Response->Append(episode.Name, " by ", episode.Show.Name, " || ", episode.IsLocal ? "local file" : returnUrl ? episode.Url : episode.Uri);
                return;
            }
            default:
            {
                Response->Append(PredefinedMessages.ListeningToAnUnknownSpotifyItemTypeMonkaS);
                return;
            }
        }
    }
}
