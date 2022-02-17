using System.Text.RegularExpressions;
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
            PlayingItem? playingItem = SpotifyRequest.GetCurrentlyPlayingTrack(ChatMessage.Username).Result;
            if (playingItem is null)
            {
                Response = $"{ChatMessage.Username}, you aren't listening to a song or aren't registered yet.";
                return;
            }
            bool channelEqualsTarget = ChatMessage.Channel.Name == ChatMessage.LowerSplit[1];
            Response = $"{ChatMessage.Username}, {SpotifyRequest.AddToQueue(ChatMessage.LowerSplit[1], playingItem.Uri, channelEqualsTarget).Result}";
            return;
        }

        pattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s\w+\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            bool channelEqualsTarget = ChatMessage.Channel.Name == ChatMessage.LowerSplit[1];
            Response = $"{ChatMessage.Username}, {SpotifyRequest.AddToQueue(ChatMessage.LowerSplit[1], ChatMessage.Split[2], channelEqualsTarget).Result}";
            return;
        }

        pattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\sme$");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            PlayingItem? playingItem = SpotifyRequest.GetCurrentlyPlayingTrack(ChatMessage.Username).Result;
            if (playingItem is null)
            {
                Response = $"{ChatMessage.Username}, you aren't listening to a song or aren't registered yet.";
                return;
            }
            Response = $"{ChatMessage.Username}, {SpotifyRequest.AddToQueue(ChatMessage.Channel.Name, playingItem.Uri, true).Result}";
            return;
        }

        pattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s\S+$");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Response = $"{ChatMessage.Username}, {SpotifyRequest.AddToQueue(ChatMessage.Channel.Name, ChatMessage.Split[1], true).Result}";
            return;
        }
    }
}
