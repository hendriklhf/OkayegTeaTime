using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Spotify;
using OkayegTeaTime.Spotify.Exceptions;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Spotify)]
public class SpotifyCommand : Command
{
    private static readonly Regex _urlPattern = new(@"^(-l)|(--url)$", RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

    public SpotifyCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        string username = ChatMessage.LowerSplit.Length > 1
            ? ChatMessage.LowerSplit[1] == "me"
                ? ChatMessage.Username
                : ChatMessage.LowerSplit[1]
            : ChatMessage.Channel;
        bool returnUrl = ChatMessage.LowerSplit.Any(s => _urlPattern.IsMatch(s));
        bool targetIsSender = username == ChatMessage.Username;
        Task.Run(async () =>
        {
            SpotifyUser? user = DbControl.SpotifyUsers[username];
            if (user is null)
            {
                Response = targetIsSender
                    ? $"{ChatMessage.Username}, can't get your currently playing song, you have to register first"
                    : $"{ChatMessage.Username}, can't get the currently playing song of {username.Antiping()}, they have to register first";
                return;
            }

            SpotifyItem? item;
            try
            {
                item = await user.GetCurrentlyPlayingItem();
            }
            catch (SpotifyException ex)
            {
                Response = $"{ChatMessage.Username}, {ex.Message}";
                return;
            }

            switch (item)
            {
                case null:
                {
                    Response = targetIsSender
                        ? $"{ChatMessage.Username}, you aren't listening to anything"
                        : $"{ChatMessage.Username}, {username.Antiping()} is not listening to anything";
                    return;
                }
                case SpotifyTrack track:
                {
                    string artists = string.Join(", ", track.Artists.Select(a => a.Name));
                    Response = $"{ChatMessage.Username}, {track.Name} by {artists} || {(track.IsLocal ? "local file" : returnUrl ? track.Url : track.Uri)}";
                    break;
                }
                case SpotifyEpisode episode:
                {
                    Response = $"{ChatMessage.Username}, {episode.Name} by {episode.Show.Name} || {(episode.IsLocal ? "local file" : returnUrl ? episode.Url : episode.Uri)}";
                    break;
                }
                default:
                {
                    Response = $"{ChatMessage.Username}, listening to an unknown Spotify item type monkaS";
                    break;
                }
            }
        }).Wait();
    }
}
