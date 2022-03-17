using System.Threading.Tasks;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Spotify;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands.CommandClasses;

public class SpotifyCommand : Command
{
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
        Response = $"{ChatMessage.Username}, ";
        bool targetIsSender = username == ChatMessage.Username;
        Task.Run(async () =>
        {
            SpotifyUser? user = DbControl.SpotifyUsers[username];
            if (user is null)
            {
                if (targetIsSender)
                {
                    Response += "can't get your currently playing song, you have to register first";
                }
                else
                {
                    Response += $"can't get the currently playing song of {username.Antiping()}, they have to register first";
                }
                return;
            }

            SpotifyItem? item = await user.GetCurrentlyPlayingItem();
            if (item is null)
            {
                if (targetIsSender)
                {
                    Response += "you aren't listening to anything";
                }
                else
                {
                    Response += $"{username.Antiping()} is not listening to anything";
                }
                return;
            }

            if (item is SpotifyTrack track)
            {
                string artists = string.Join(", ", track.Artists.Select(a => a.Name));
                Response += $"{track.Name} by {artists} || {(track.IsLocal ? "local file" : track.Uri)}";
            }
            else if (item is SpotifyEpisode episode)
            {
                Response += $"{episode.Name} by {episode.Show.Name} || {(episode.IsLocal ? "local file" : episode.Uri)}";
            }
            else
            {
                Response += "listening to an unknown Spotify item type monkaS";
            }
        }).Wait();
    }
}
