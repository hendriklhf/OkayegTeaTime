using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Spotify;
using OkayegTeaTime.Spotify.Exceptions;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

public class SongRequestCommand : Command
{
    public SongRequestCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex pattern = PatternCreator.Create(Alias, Prefix, @"\s\w+\sme");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Task.Run(async () =>
            {
                SpotifyUser? user = DbControl.SpotifyUsers[ChatMessage.Username];
                if (user is null)
                {
                    Response = $"{ChatMessage.Username}, you aren't registered yet";
                    return;
                }

                SpotifyItem? playingItem;
                try
                {
                    playingItem = await user.GetCurrentlyPlayingItem();
                }
                catch (SpotifyException ex)
                {
                    Response = $"{ChatMessage.Username}, {ex.Message}";
                    return;
                }

                if (playingItem is null)
                {
                    Response = $"{ChatMessage.Username}, you aren't listening to a song";
                    return;
                }

                SpotifyUser? target = DbControl.SpotifyUsers[ChatMessage.LowerSplit[1]];
                if (target is null)
                {
                    Response = $"{ChatMessage.Username}, {ChatMessage.LowerSplit[1].Antiping()} isn't registered yet, they have to register first";
                    return;
                }

                try
                {
                    SpotifyItem item = await target.AddToQueue(playingItem.Uri);
                    if (item is SpotifyTrack track)
                    {
                        string artists = string.Join(", ", track.Artists.Select(a => a.Name));
                        Response = $"{ChatMessage.Username}, {track.Name} by {artists} || {(track.IsLocal ? "local file" : track.Uri)} has been added to the queue of {target.Username.Antiping()}";
                    }
                    else if (item is SpotifyEpisode episode)
                    {
                        Response = $"{ChatMessage.Username}, {episode.Name} by {episode.Show.Name} || {(episode.IsLocal ? "local file" : episode.Uri)} has been added to the queue of " +
                                   $"{target.Username.Antiping()}";
                    }
                }
                catch (SpotifyException ex)
                {
                    Response = $"{ChatMessage.Username}, {ex.Message}";
                }
            }).Wait();
            return;
        }

        pattern = PatternCreator.Create(Alias, Prefix, @"\sme\s\w+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Task.Run(async () =>
            {
                SpotifyUser? user = DbControl.SpotifyUsers[ChatMessage.Username];
                if (user is null)
                {
                    Response = $"{ChatMessage.Username}, you aren't registered yet, you have to register first";
                    return;
                }

                SpotifyUser? target = DbControl.SpotifyUsers[ChatMessage.LowerSplit[2]];
                if (target is null)
                {
                    Response = $"{ChatMessage.Username}, {ChatMessage.LowerSplit[2].Antiping()} isn't registered yet, they have to register first";
                    return;
                }

                SpotifyItem? currentlyPlaying;
                try
                {
                    currentlyPlaying = await target.GetCurrentlyPlayingItem();
                }
                catch (SpotifyException ex)
                {
                    Response = $"{ChatMessage.Username}, {ex.Message}";
                    return;
                }

                if (currentlyPlaying is null)
                {
                    Response = $"{ChatMessage.Username}, {ChatMessage.LowerSplit[2].Antiping()} isn't listening to anything";
                    return;
                }

                try
                {
                    SpotifyItem item = await user.AddToQueue(currentlyPlaying.Uri);
                    if (item is SpotifyTrack track)
                    {
                        string artists = string.Join(", ", track.Artists.Select(a => a.Name));
                        Response = $"{ChatMessage.Username}, {track.Name} by {artists} || {(track.IsLocal ? "local file" : track.Uri)} has been added to the queue of {user.Username.Antiping()}";
                    }
                    else if (item is SpotifyEpisode episode)
                    {
                        Response = $"{ChatMessage.Username}, {episode.Name} by {episode.Show.Name} || {(episode.IsLocal ? "local file" : episode.Uri)} has been added to the queue of " +
                                   $"{user.Username.Antiping()}";
                    }
                }
                catch (SpotifyException ex)
                {
                    Response = $"{ChatMessage.Username}, {ex.Message}";
                }
            }).Wait();
            return;
        }

        pattern = PatternCreator.Create(Alias, Prefix, @"\s\w+\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Task.Run(async () =>
            {
                SpotifyUser? target = DbControl.SpotifyUsers[ChatMessage.LowerSplit[1]];
                if (target is null)
                {
                    Response = $"{ChatMessage.Username}, {ChatMessage.LowerSplit[1].Antiping()} isn't registered yet, they have to register first";
                    return;
                }

                try
                {
                    SpotifyItem item = await target.AddToQueue(ChatMessage.Split[2]);
                    if (item is SpotifyTrack track)
                    {
                        string artists = string.Join(", ", track.Artists.Select(a => a.Name));
                        Response = $"{ChatMessage.Username}, {track.Name} by {artists} || {(track.IsLocal ? "local file" : track.Uri)} has been added to the queue of {target.Username.Antiping()}";
                    }
                    else if (item is SpotifyEpisode episode)
                    {
                        Response = $"{ChatMessage.Username}, {episode.Name} by {episode.Show.Name} || {(episode.IsLocal ? "local file" : episode.Uri)} has been added to the queue of " +
                                   $"{target.Username.Antiping()}";
                    }
                }
                catch (SpotifyException ex)
                {
                    Response = $"{ChatMessage.Username}, {ex.Message}";
                }
            }).Wait();
            return;
        }

        pattern = PatternCreator.Create(Alias, Prefix, @"\sme$");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Task.Run(async () =>
            {
                SpotifyUser? user = DbControl.SpotifyUsers[ChatMessage.Username];
                if (user is null)
                {
                    Response = $"{ChatMessage.Username}, you aren't registered yet, you have to register first";
                    return;
                }

                SpotifyItem? playingItem;
                try
                {
                    playingItem = await user.GetCurrentlyPlayingItem();
                }
                catch (SpotifyException ex)
                {
                    Response = $"{ChatMessage.Username}, {ex.Message}";
                    return;
                }

                if (playingItem is null)
                {
                    Response = $"{ChatMessage.Username}, you aren't listening to anything";
                    return;
                }

                SpotifyUser? target = DbControl.SpotifyUsers[ChatMessage.Channel];
                if (target is null)
                {
                    Response = $"{ChatMessage.Username}, {ChatMessage.Channel.Antiping()} isn't registered yet, they have to register first";
                    return;
                }

                try
                {
                    SpotifyItem item = await target.AddToQueue(playingItem.Uri);
                    if (item is SpotifyTrack track)
                    {
                        string artists = string.Join(", ", track.Artists.Select(a => a.Name));
                        Response = $"{ChatMessage.Username}, {track.Name} by {artists} || {(track.IsLocal ? "local file" : track.Uri)} has been added to the queue of {target.Username.Antiping()}";
                    }
                    else if (item is SpotifyEpisode episode)
                    {
                        Response = $"{ChatMessage.Username}, {episode.Name} by {episode.Show.Name} || {(episode.IsLocal ? "local file" : episode.Uri)} has been added to the queue of " +
                                   $"{target.Username.Antiping()}";
                    }
                }
                catch (SpotifyException ex)
                {
                    Response = $"{ChatMessage.Username}, {ex.Message}";
                }
            }).Wait();
            return;
        }

        pattern = PatternCreator.Create(Alias, Prefix, @"\s\S+$");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Task.Run(async () =>
            {
                SpotifyUser? target = DbControl.SpotifyUsers[ChatMessage.Channel];
                if (target is null)
                {
                    Response = $"{ChatMessage.Username}, {ChatMessage.Channel.Antiping()} isn't registered yet, they have to register first";
                    return;
                }

                try
                {
                    SpotifyItem item = await target.AddToQueue(ChatMessage.Split[1]);
                    if (item is SpotifyTrack track)
                    {
                        string artists = string.Join(", ", track.Artists.Select(a => a.Name));
                        Response = $"{ChatMessage.Username}, {track.Name} by {artists} || {(track.IsLocal ? "local file" : track.Uri)} has been added to the queue of {target.Username.Antiping()}";
                    }
                    else if (item is SpotifyEpisode episode)
                    {
                        Response = $"{ChatMessage.Username}, {episode.Name} by {episode.Show.Name} || {(episode.IsLocal ? "local file" : episode.Uri)} has been added to the queue of " +
                                   $"{target.Username.Antiping()}";
                    }
                }
                catch (SpotifyException ex)
                {
                    Response = $"{ChatMessage.Username}, {ex.Message}";
                }
            }).Wait();
        }
    }
}
