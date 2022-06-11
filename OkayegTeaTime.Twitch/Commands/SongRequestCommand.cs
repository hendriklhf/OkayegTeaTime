using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE.Collections;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Spotify;
using OkayegTeaTime.Spotify.Exceptions;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

public class SongRequestCommand : Command
{
    private static readonly Regex _targetPattern = new($@"^\S+\s{Pattern.MultipleReminderTargets}", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));
    
    public SongRequestCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex pattern = PatternCreator.Create(Alias, Prefix, @"\s\w+\sme");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            string[] targets = GetTargets();
            
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

                string[] unregisteredTargets;
                for (int i = 0; i < targets.Length; i++)
                {
                    SpotifyUser? user = DbControl.SpotifyUsers[targets[i]];
                    if (user is null)
                    { 
                        unregisteredTargets = targets[i];
                    }
                }

                FilterTargets(targets, unregisteredTargets);
                string target;
                string unregisteredTarget;
                try
                {
                    SpotifyItem item = await target.AddToQueue(playingItem.Uri);
                    if (item is SpotifyTrack track)
                    {
                        string artists = string.Join(", ", track.Artists.Select(a => a.Name));
                        if (unregisteredTargets.Any())
                        {
                            if (targets.Length == 1)
                            {
                                Response = $"{ChatMessage.Username}, {track.Name} by {artists} || {(track.IsLocal ? "local file" : track.Uri)} has been added to the queue of {targets[0]}";
                            }
                            else
                            {
                                targets.ForEach(d => target += $"{d}, ");
                                Response = $"{ChatMessage.Username}, {track.Name} by {artists} || {(track.IsLocal ? "local file" : track.Uri)} has been added to the queue of {target}";
                            }
                        }
                        else
                        {
                            if (unregisteredTarget.Length != 1)
                            {
                                targets.ForEach(d => target += $"{d}, ");
                                unregisteredTargets.ForEach(d => unregisteredTarget += $"{d}, ")
                                Response = $"{ChatMessage.Username}, {track.Name} by {artists} || {(track.IsLocal ? "local file" : track.Uri)} has been added to the queue of {target}. The users {unregisteredTarget} aren't registered yet.";
                            }
                            else
                            {
                                Response = $"{ChatMessage.Username}, {track.Name} by {artists} || {(track.IsLocal ? "local file" : track.Uri)} has been added to the queue of {targets[0]}. The user {unregisteredTarget[0]} isn't registered yet.";
                            }
                        }
                    }
                    else if (item is SpotifyEpisode episode)
                    {
                        Response = $"{ChatMessage.Username}, {episode.Name} by {episode.Show.Name} || {(episode.IsLocal ? "local file" : episode.Uri)} has been added to the queue of " +
                                   $"{target.Username.Antiping()}";
                        if (unregisteredTargets.Any())
                        { 
                            if (targets.Length == 1)
                            {
                                Response = $"{ChatMessage.Username}, {episode.Name} by {episode.Show.Name} || {(episode.IsLocal ? "local file" : episode.Uri)} has been added to the queue of " +
                                           $"{targets[0]}";
                            }
                            else
                            {
                                targets.ForEach(d => target += $"{d}, ");
                                Response = $"{ChatMessage.Username}, {episode.Name} by {episode.Show.Name} || {(episode.IsLocal ? "local file" : episode.Uri)} has been added to the queue of " +
                                           $"{target}";#
                            }
                        }
                        else
                        {
                            if (unregisteredTarget.Length != 1)
                            {
                                targets.ForEach(d => target += $"{d}, ");
                                unregisteredTargets.ForEach(d => unregisteredTarget += $"{d}, ")
                                Response = $"{ChatMessage.Username}, {track.Name} by {artists} || {(track.IsLocal ? "local file" : track.Uri)} has been added to the queue of {target}. The users {unregisteredTarget} aren't registered yet.";
                            }
                            else
                            {
                                Response = $"{ChatMessage.Username}, {track.Name} by {artists} || {(track.IsLocal ? "local file" : track.Uri)} has been added to the queue of {target[0]}. The user {unregisteredTarget[0]} isn't registered yet.";
                            }
                            
                        }
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
                string[] unregisteredTargets;
                for (int i = 0; i < targets.Length; i++)
                {
                    SpotifyUser? user = DbControl.SpotifyUsers[targets[i]];
                    if (user is null)
                    { 
                        unregisteredTargets = targets[i];
                    }
                }

                FilterTargets(targets, unregisteredTargets);
                string target;
                string unregisteredTarget;
                try
                {
                    SpotifyItem item = await target.AddToQueue(ChatMessage.Split[2..].JoinToString(' '));
                    if (item is SpotifyTrack track)
                    {
                        string artists = string.Join(", ", track.Artists.Select(a => a.Name));
                        if (unregisteredTargets.Any())
                        {
                            if (targets.Length == 1)
                            {
                                Response = $"{ChatMessage.Username}, {track.Name} by {artists} || {(track.IsLocal ? "local file" : track.Uri)} has been added to the queue of {targets[0]}";
                            }
                            else
                            {
                                targets.ForEach(d => target += $"{d}, ");
                                Response = $"{ChatMessage.Username}, {track.Name} by {artists} || {(track.IsLocal ? "local file" : track.Uri)} has been added to the queue of {target}";
                            }
                        }
                        else
                        {
                            if (unregisteredTarget.Length != 1)
                            {
                                targets.ForEach(d => target += $"{d}, ");
                                unregisteredTargets.ForEach(d => unregisteredTarget += $"{d}, ")
                                Response = $"{ChatMessage.Username}, {track.Name} by {artists} || {(track.IsLocal ? "local file" : track.Uri)} has been added to the queue of {target}. The users {unregisteredTarget} aren't registered yet.";
                            }
                            else
                            {
                                Response = $"{ChatMessage.Username}, {track.Name} by {artists} || {(track.IsLocal ? "local file" : track.Uri)} has been added to the queue of {targets[0]}. The user {unregisteredTarget[0]} isn't registered yet.";
                            }
                        }
                    }
                    else if (item is SpotifyEpisode episode)
                    {
                        Response = $"{ChatMessage.Username}, {episode.Name} by {episode.Show.Name} || {(episode.IsLocal ? "local file" : episode.Uri)} has been added to the queue of " +
                                   $"{target.Username.Antiping()}";
                        if (unregisteredTargets.Any())
                        { 
                            if (targets.Length == 1)
                            {
                                Response = $"{ChatMessage.Username}, {episode.Name} by {episode.Show.Name} || {(episode.IsLocal ? "local file" : episode.Uri)} has been added to the queue of " +
                                           $"{targets[0]}";
                            }
                            else
                            {
                                targets.ForEach(d => target += $"{d}, ");
                                Response = $"{ChatMessage.Username}, {episode.Name} by {episode.Show.Name} || {(episode.IsLocal ? "local file" : episode.Uri)} has been added to the queue of " +
                                           $"{target}";#
                            }
                        }
                        else
                        {
                            if (unregisteredTarget.Length != 1)
                            {
                                targets.ForEach(d => target += $"{d}, ");
                                unregisteredTargets.ForEach(d => unregisteredTarget += $"{d}, ")
                                Response = $"{ChatMessage.Username}, {track.Name} by {artists} || {(track.IsLocal ? "local file" : track.Uri)} has been added to the queue of {target}. The users {unregisteredTarget} aren't registered yet.";
                            }
                            else
                            {
                                Response = $"{ChatMessage.Username}, {track.Name} by {artists} || {(track.IsLocal ? "local file" : track.Uri)} has been added to the queue of {target[0]}. The user {unregisteredTarget[0]} isn't registered yet.";
                            }
                            
                        }
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

        pattern = PatternCreator.Create(Alias, Prefix, @"\s\S+");
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
                    SpotifyItem item = await target.AddToQueue(ChatMessage.Split[1..].JoinToString(' '));
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
    
    private string[] GetTargets()
    {
        Match match = _targetPattern.Match(ChatMessage.Message);
        int firstWordLength = match.Value.Split()[0].Length + 1;
        string[] targets = match.Value[firstWordLength..].Remove(" ").Split(',');
        return targets.Replace(t => t.ToLower() == "me", ChatMessage.Username)
            .Select(t => t.ToLower())
            .Distinct()
            .Take(5)
            .ToArray();
    }

    private string[] FilterTargets(string[] targets, string[] unregisteredTargets)
    {
        for (int i = 0; i < targets.Length; i++)
        {
            if (unregisteredTargets.Any(targets[i]))
            {
                targets = targets.Where(e => e != targets[i]).ToArray();
            }
        }
    }
}
