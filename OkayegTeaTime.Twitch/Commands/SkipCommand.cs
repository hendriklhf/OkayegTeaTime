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
                await SpotifyController.Skip(user);
                Response = $"{ChatMessage.Username}, skipped to the next song in {ChatMessage.Channel.Antiping()}'s queue";
            }
            catch (SpotifyException ex)
            {
                Response = $"{ChatMessage.Username}, {ex.Message}";
            }

            List<SpotifyUser> usersToRemove = new();
            ListeningSession? listeningSession = SpotifyController.GetListeningSession(user);
            if (listeningSession is null)
            {
                return;
            }

            foreach (SpotifyUser listener in listeningSession.Listeners)
            {
                try
                {
                    await SpotifyController.ListenAlongWith(listener, user);
                }
                catch (SpotifyException)
                {
                    usersToRemove.Add(listener);
                }
            }

            foreach (SpotifyUser u in usersToRemove)
            {
                listeningSession.Listeners.Remove(u);
            }
        }).Wait();
    }
}
