using System.Collections.Generic;
using System.Threading.Tasks;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Spotify;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Skip)]
public sealed class SkipCommand : Command
{
    public SkipCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias) : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        if (!ChatMessage.IsModerator && !ChatMessage.IsBroadcaster)
        {
            Response = $"{ChatMessage.Username}, {PredefinedMessages.NoModOrBroadcasterMessage}";
            return;
        }

        Task.Run(async () =>
        {
            SpotifyUser? user = _twitchBot.SpotifyUsers[ChatMessage.Channel];
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

            List<SpotifyUser> usersToRemove = new();
            foreach (SpotifyUser u in user.ListeningUsers)
            {
                try
                {
                    await u.ListenAlongWith(user);
                }
                catch (SpotifyException)
                {
                    usersToRemove.Add(u);
                }
            }

            foreach (SpotifyUser u in usersToRemove)
            {
                user.ListeningUsers.Remove(u);
            }
        }).Wait();
    }
}
