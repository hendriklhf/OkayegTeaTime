using System.Threading.Tasks;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Spotify.Exceptions;
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
        Regex pattern = PatternCreator.Create(Alias, Prefix);
        if (pattern.IsMatch(ChatMessage.Message))
        {
            if (!ChatMessage.IsModerator && !ChatMessage.IsBroadcaster)
            {
                Response = $"{ChatMessage.Username}, {PredefinedMessages.NoModOrBroadcasterMessage}";
            }

            Task.Run(async () =>
            {
                SpotifyUser? user = DbControl.SpotifyUsers[ChatMessage.Channel];
                if (user is null)
                {
                    Response = $"{ChatMessage.Username}, you can't skip songs of {ChatMessage.Channel.Antiping()}, they have to register first";
                    return;
                }

                try
                {
                    await user.Skip();
                    Response = $"{ChatMessage.Username}, skipped to the next song in {ChatMessage.Channel.Antiping()}'s queue";
                }
                catch (SpotifyException ex)
                {
                    Response = $"{ChatMessage.Username}, {ex.Message}";
                }

                foreach (SpotifyUser u in user.ListeningUsers)
                {
                    try
                    {
                        await u.ListenAlongWith(user);
                    }
                    catch (SpotifyException)
                    {
                        user.ListeningUsers.Remove(u);
                    }
                }
            }).Wait();
        }
    }
}
