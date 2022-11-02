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
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.SongRequest)]
public sealed class SongRequestCommand : Command
{
    private static readonly Regex _exceptTargetPattern = new($@"^\S+\s{Pattern.MultipleTargets}\s", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

    public SongRequestCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias) : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        #region sr {users...} me

        Regex pattern = PatternCreator.Create(_alias, _prefix, $@"\s{Pattern.MultipleTargets}\sme");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Task.Run(async () =>
            {
                SpotifyUser? user = _twitchBot.SpotifyUsers[ChatMessage.Username];
                if (user is null)
                {
                    Response = $"{ChatMessage.Username}, you aren't registered yet";
                    return;
                }

                SpotifyItem? item;
                try
                {
                    item = await SpotifyController.GetCurrentlyPlayingItem(user);
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

                SpotifyTrack track = (SpotifyTrack)item;
                Dictionary<SpotifyUser, string?> success = new();
                foreach (SpotifyUser target in targets)
                {
                    try
                    {
                        await SpotifyController.AddToQueue(target, track.Uri);
                        success.Add(target, null);
                    }
                    catch (SpotifyException ex)
                    {
                        success.Add(target, ex.Message);
                        DbController.LogException(ex);
                    }
                }

                CreateMultipleTargetResponse(success, track);
            }).Wait();
            return;
        }

        #endregion sr {user} me

        #region sr me {user}

        pattern = PatternCreator.Create(_alias, _prefix, @"\sme\s\w+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Task.Run(async () =>
            {
                SpotifyUser? user = _twitchBot.SpotifyUsers[ChatMessage.Username];
                if (user is null)
                {
                    Response = $"{ChatMessage.Username}, you aren't registered yet, you have to register first";
                    return;
                }

                SpotifyUser? target = _twitchBot.SpotifyUsers[ChatMessage.LowerSplit[2]];
                if (target is null)
                {
                    Response = $"{ChatMessage.Username}, {ChatMessage.LowerSplit[2].Antiping()} isn't registered yet, they have to register first";
                    return;
                }

                SpotifyItem? item;
                try
                {
                    item = await SpotifyController.GetCurrentlyPlayingItem(target);
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
                    await SpotifyController.AddToQueue(user, item.Uri);
                }
                catch (SpotifyException ex)
                {
                    Response = $"{ChatMessage.Username}, {ex.Message}";
                    return;
                }

                SpotifyTrack track = (item as SpotifyTrack)!;
                Response = $"{ChatMessage.Username}, {track} || {(track.IsLocal ? "local file" : track.Uri)} has been added to the queue of {user.Username.Antiping()}";
            }).Wait();
            return;
        }

        #endregion

        #region sr {users...} {song}

        pattern = PatternCreator.Create(_alias, _prefix, $@"\s{Pattern.MultipleTargets}\s\S+");
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
                    track = await SpotifyController.SearchTrack(targets[0], song);
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

                Dictionary<SpotifyUser, string?> success = new();
                foreach (SpotifyUser target in targets)
                {
                    try
                    {
                        await SpotifyController.AddToQueue(target, track.Uri);
                        success.Add(target, null);
                    }
                    catch (SpotifyException ex)
                    {
                        success.Add(target, ex.Message);
                        DbController.LogException(ex);
                    }
                }

                CreateMultipleTargetResponse(success, track);
            }).Wait();
            return;
        }

        #endregion sr {user} {song}

        #region sr me

        pattern = PatternCreator.Create(_alias, _prefix, @"\sme$");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Task.Run(async () =>
            {
                SpotifyUser? user = _twitchBot.SpotifyUsers[ChatMessage.Username];
                if (user is null)
                {
                    Response = $"{ChatMessage.Username}, you aren't registered yet, you have to register first";
                    return;
                }

                SpotifyItem? item;
                try
                {
                    item = await SpotifyController.GetCurrentlyPlayingItem(user);
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

                SpotifyUser? target = _twitchBot.SpotifyUsers[ChatMessage.Channel];
                if (target is null)
                {
                    Response = $"{ChatMessage.Username}, {ChatMessage.Channel.Antiping()} isn't registered yet, they have to register first";
                    return;
                }

                try
                {
                    await SpotifyController.AddToQueue(target, item.Uri);
                }
                catch (SpotifyException ex)
                {
                    Response = $"{ChatMessage.Username}, {ex.Message}";
                    return;
                }

                SpotifyTrack track = (item as SpotifyTrack)!;
                Response = $"{ChatMessage.Username}, {track} || {(track.IsLocal ? "local file" : track.Uri)} has been added to the queue of {target.Username.Antiping()}";
            }).Wait();
            return;
        }

        #endregion sr me

        #region sr {song}

        pattern = PatternCreator.Create(_alias, _prefix, @"\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Task.Run(async () =>
            {
                SpotifyUser? target = _twitchBot.SpotifyUsers[ChatMessage.Channel];
                if (target is null)
                {
                    Response = $"{ChatMessage.Username}, {ChatMessage.Channel.Antiping()} isn't registered yet, they have to register first";
                    return;
                }

                try
                {
                    SpotifyTrack track = await SpotifyController.AddToQueue(target, ChatMessage.Message[(ChatMessage.Split[0].Length + 1)..]);
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
        Match match = Pattern.MultipleTargets.Match(ChatMessage.LowerSplit[1..^1].JoinToString(' '));
        string[] targets = match.Value.Split(',');
        return targets.Select(t => t.TrimAll()).Distinct().Select(t => _twitchBot.SpotifyUsers[t]).Where(t => t is not null).Take(5).ToArray()!;
    }

    private void CreateMultipleTargetResponse(Dictionary<SpotifyUser, string?> success, SpotifyTrack track)
    {
        string[] successUsers = success.Where(t => t.Value is null).Select(t => t.Key.Username.Antiping()).ToArray();
        string[] failedUsers = success.Where(t => t.Value is not null).Select(t => t.Value!).ToArray();

        if (successUsers.Length > 0)
        {
            Response += $"{ChatMessage.Username}, {track} || {(track.IsLocal ? "local file" : track.Uri)} has been added to the queue";
            Response += successUsers.Length > 1 ? "s of " : " of ";
            Response += successUsers.JoinToString(", ");
            if (failedUsers.Length == 0)
            {
                return;
            }

            Response += ". ";
            Response += failedUsers.JoinToString(", ");
        }
        else
        {
            if (failedUsers.Length == 1)
            {
                Response = $"{ChatMessage.Username}, {failedUsers[0]}";
            }
            else
            {
                Response += $"{ChatMessage.Username}, {track} || {(track.IsLocal ? "local file" : track.Uri)} hasn't been added to any queue";
                if (failedUsers.Length == 0)
                {
                    return;
                }

                Response += ". ";
                Response += failedUsers.JoinToString(", ");
            }
        }
    }
}
