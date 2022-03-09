using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OkayegTeaTime.Spotify;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands.CommandClasses;

public class SongRequestCommand : Command
{
    public SongRequestCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex pattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s\w+\sme(\s|$)");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Task.Run(async () =>
            {
                SpotifyUser? user = await SpotifyController.GetSpotifyUser(ChatMessage.Username);
                if (user is null)
                {
                    Response = $"{ChatMessage.Username}, you aren't registered yet";
                    return;
                }

                SpotifyItem? playingItem = await user.GetCurrentlyPlayingItem();
                if (playingItem is null)
                {
                    Response = $"{ChatMessage.Username}, you aren't listening to a song";
                    return;
                }

                SpotifyUser? target = await SpotifyController.GetSpotifyUser(ChatMessage.LowerSplit[1]);
                if (target is null)
                {
                    Response = $"{ChatMessage.Username}, {ChatMessage.LowerSplit[1].Antiping()} isn't registered yet, they have to register first";
                    return;
                }

                await target.AddToQueue(playingItem.Uri);
                Response = $"{ChatMessage.Username}, {target.Response}";
            }).Wait();
            return;
        }

        pattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\sme\s\w+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Task.Run(async () =>
            {
                SpotifyUser? user = await SpotifyController.GetSpotifyUser(ChatMessage.Username);
                if (user is null)
                {
                    Response = $"{ChatMessage.Username}, you aren't registered yet, you have to register first";
                    return;
                }

                SpotifyUser? target = await SpotifyController.GetSpotifyUser(ChatMessage.LowerSplit[2]);
                if (target is null)
                {
                    Response = $"{ChatMessage.Username}, {ChatMessage.LowerSplit[2].Antiping()} isn't registered yet, they have to register first";
                    return;
                }

                SpotifyItem? currentlyPlaying = await target.GetCurrentlyPlayingItem();
                if (currentlyPlaying is null)
                {
                    Response = $"{ChatMessage.Username}, {ChatMessage.LowerSplit[2].Antiping()} isn't listening to anything";
                    return;
                }

                await user.AddToQueue(currentlyPlaying.Uri);
                Response = $"{ChatMessage.Username}, {user.Response}";
            }).Wait();
            return;
        }

        pattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s\w+\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Task.Run(async () =>
            {
                SpotifyUser? user = await SpotifyController.GetSpotifyUser(ChatMessage.LowerSplit[1]);
                if (user is null)
                {
                    Response = $"{ChatMessage.Username}, {ChatMessage.LowerSplit[1].Antiping()} isn't registered yet, they have to register first";
                    return;
                }

                await user.AddToQueue(ChatMessage.Split[2]);
                Response = $"{ChatMessage.Username}, {user.Response}";
            }).Wait();
            return;
        }

        pattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\sme$");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Task.Run(async () =>
            {
                SpotifyUser? user = await SpotifyController.GetSpotifyUser(ChatMessage.Username);
                if (user is null)
                {
                    Response = $"{ChatMessage.Username}, you aren't registered yet, you have to register first";
                    return;
                }

                SpotifyItem? playingItem = await user.GetCurrentlyPlayingItem();
                if (playingItem is null)
                {
                    Response = $"{ChatMessage.Username}, you aren't listening to anything";
                    return;
                }

                SpotifyUser? target = await SpotifyController.GetSpotifyUser(ChatMessage.Channel.Name);
                if (target is null)
                {
                    Response = $"{ChatMessage.Username}, {ChatMessage.Channel.Name.Antiping()} isn't registered yet, they have to register first";
                    return;
                }

                await target.AddToQueue(playingItem.Uri);
                Response = $"{ChatMessage.Username}, {target.Response}";
            }).Wait();
            return;
        }

        pattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s\S+$");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Task.Run(async () =>
            {
                SpotifyUser? user = await SpotifyController.GetSpotifyUser(ChatMessage.Channel.Name);
                if (user is null)
                {
                    Response = $"{ChatMessage.Username}, {ChatMessage.Channel.Name.Antiping()} isn't registered yet, they have to register first";
                    return;
                }

                await user.AddToQueue(ChatMessage.Split[1]);
                Response = $"{ChatMessage.Username}, {user.Response}";
            }).Wait();
            return;
        }
    }
}
