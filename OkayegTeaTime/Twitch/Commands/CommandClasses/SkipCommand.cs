using System.Text.RegularExpressions;
using OkayegTeaTime.Spotify;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands.CommandClasses;

public class SkipCommand : Command
{
    public SkipCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex pattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix);
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Response = $"{ChatMessage.Username}, ";
            if (ChatMessage.IsModerator || ChatMessage.IsBroadcaster)
            {
                Response += SpotifyRequest.SkipToNextSong(ChatMessage.Channel.Name).Result;
            }
            else
            {
                Response += "you have to be a mod or the broadcaster to skip the song";
            }
        }
    }
}
