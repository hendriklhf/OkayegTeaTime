using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE;
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
    private static readonly Regex _targetPattern = new(Pattern.MultipleTargets, RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));
    private static readonly Regex _exceptTargetPattern = new($@"^\S+\s{Pattern.MultipleTargets}\s", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));


    public SongRequestCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        #region sr {users...} me

        Regex pattern = PatternCreator.Create(Alias, Prefix, $@"\s{Pattern.MultipleTargets}\sme");
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

                SpotifyItem? item;
                try
                {
                    item = await user.GetCurrentlyPlayingItem();
                }
                catch (SpotifyException ex)
                {
                    Response = $"{ChatMessage.Username}, {ex.Message}";
                    return;
                }

                if (item is null or not SpotifyTrack)
                {
                    Response = $"{ChatMessage.Username}, you aren't listening to a song";
                    return;
                }

                SpotifyUser[] targets = GetTargets();
                if (targets.Length == 0)
                {
                    Response = $"{ChatMessage.Username}, none of the given users are registered";
                    return;
                }

                SpotifyTrack track = (item as SpotifyTrack)!;
                Response = $"{ChatMessage.Username}, {track} || {(track.IsLocal ? "local file" : track.Uri)} has been added to the queue";

                Response += targets.Length > 1 ? "s of " : " of ";
                Dictionary<SpotifyUser, bool> success = new();
                foreach (SpotifyUser target in targets)
                {
                    try
                    {
                        await target.AddToQueue(track.Uri);
                        success.Add(target, true);
                    }
                    catch (SpotifyException ex)
                    {
                        success.Add(target, false);
                        DbController.LogException(ex);
                    }
                }

                Response += success.Where(t => t.Value).Select(t => t.Key.Username.Antiping()).JoinToString(", ");
            }).Wait();
            return;
        }

        #endregion sr {user} me

        #region sr me {user}

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

                SpotifyItem? item;
                try
                {
                    item = await target.GetCurrentlyPlayingItem();
                }
                catch (SpotifyException ex)
                {
                    Response = $"{ChatMessage.Username}, {ex.Message}";
                    return;
                }

                if (item is null or not SpotifyTrack)
                {
                    Response = $"{ChatMessage.Username}, {ChatMessage.LowerSplit[2].Antiping()} isn't listening to a track";
                    return;
                }

                try
                {
                    await user.AddToQueue(item.Uri);
                }
                catch (SpotifyException ex)
                {
                    Response = $"{ChatMessage.Username}, {ex.Message}";
                }

                SpotifyTrack track = (item as SpotifyTrack)!;
                Response = $"{ChatMessage.Username}, {track} || {(track.IsLocal ? "local file" : track.Uri)} has been added to the queue of {user.Username.Antiping()}";
            }).Wait();
            return;
        }

        #endregion

        #region sr {users...} {song}

        pattern = PatternCreator.Create(Alias, Prefix, $@"\s{Pattern.MultipleTargets}\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Task.Run(async () =>
            {
                SpotifyUser[] targets = GetTargets();
                if (targets.Length == 0)
                {
                    Response = $"{ChatMessage.Username}, none of the given users are registered";
                    return;
                }

                string song = _exceptTargetPattern.Replace(ChatMessage.Message, string.Empty);
                SpotifyTrack? track = null;
                try
                {
                    track = await targets[0].SearchTrack(song);
                }
                catch (SpotifyException ex)
                {
                    Response = $"{ChatMessage.Username}, {ex.Message}";
                }

                if (track is null)
                {
                    Response = $"{ChatMessage.Username}, no matching track could be found";
                    return;
                }

                Response = $"{ChatMessage.Username}, {track} || {(track.IsLocal ? "local file" : track.Uri)} has been added to the queue";
                Response += targets.Length > 1 ? "s of " : " of ";
                Dictionary<SpotifyUser, bool> success = new();
                foreach (SpotifyUser target in targets)
                {
                    try
                    {
                        await target.AddToQueue(track.Uri);
                        success.Add(target, true);
                    }
                    catch (SpotifyException ex)
                    {
                        success.Add(target, false);
                        DbController.LogException(ex);
                    }
                }

                Response += success.Where(t => t.Value).Select(t => t.Key.Username.Antiping()).JoinToString(", ");
            }).Wait();
            return;
        }

        #endregion sr {user} {song}

        #region sr me

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

                SpotifyItem? item;
                try
                {
                    item = await user.GetCurrentlyPlayingItem();
                }
                catch (SpotifyException ex)
                {
                    Response = $"{ChatMessage.Username}, {ex.Message}";
                    return;
                }

                if (item is null or not SpotifyTrack)
                {
                    Response = $"{ChatMessage.Username}, you aren't listening to a track";
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
                    await target.AddToQueue(item.Uri);
                }
                catch (SpotifyException ex)
                {
                    Response = $"{ChatMessage.Username}, {ex.Message}";
                }

                SpotifyTrack track = (item as SpotifyTrack)!;
                Response = $"{ChatMessage.Username}, {track} || {(track.IsLocal ? "local file" : track.Uri)} has been added to the queue of {target.Username.Antiping()}";
            }).Wait();
            return;
        }

        #endregion sr me

        #region sr {song}

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
                    SpotifyTrack track = await target.AddToQueue(ChatMessage.Split[1..].JoinToString(' '));
                    Response = $"{ChatMessage.Username}, {track} || {(track.IsLocal ? "local file" : track.Uri)} has been added to the queue of {target.Username.Antiping()}";
                }
                catch (SpotifyException ex)
                {
                    Response = $"{ChatMessage.Username}, {ex.Message}";
                }
            }).Wait();
        }

        #endregion sr {song}
    }

    private SpotifyUser[] GetTargets()
    {
        Match match = _targetPattern.Match(ChatMessage.LowerSplit[1..^1].JoinToString(' '));
        string[] targets = match.Value.Split(',');
        return targets.Select(t => t.TrimAll()).Distinct()
            .Select(t => DbControl.SpotifyUsers[t])
            .Where(t => t is not null).Take(5).ToArray()!;
    }
}
