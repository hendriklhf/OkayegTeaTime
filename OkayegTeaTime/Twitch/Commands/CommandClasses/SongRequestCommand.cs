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
        Regex pattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s\S+$");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Response = $"{ChatMessage.Username}, {SpotifyRequest.AddToQueue(ChatMessage.Channel.Name, ChatMessage.Split[1]).Result}";
            return;
        }

        pattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, "\s\w+\s\w+");
        if(pattern.IsMatch(ChatMessage.Message))
        {
            if (DbController.DoesSpotifyUserExist(ChatMessage.Split[2]))
            {
                Response = $"{ChatMessage.Username}, {SpotifyRequest.AddToQueue(ChatMessage.LowerSplit[1], SpotifyRequest.GetCurrentlyPlayingTrack(ChatMessage.Split[2]), false).Result};
            }
        }

        pattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s\w+\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Response = $"{ChatMessage.Username}, {SpotifyRequest.AddToQueue(ChatMessage.LowerSplit[1], ChatMessage.Split[2], false).Result}";
        }
    }
}
