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
using StringHelper = HLE.StringHelper;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.SongRequest)]
public readonly unsafe ref struct SongRequestCommand
{
    public TwitchChatMessage ChatMessage { get; }

    public Response* Response { get; }

    private readonly TwitchBot _twitchBot;
    private readonly string? _prefix;
    private readonly string _alias;

    private static readonly Regex _exceptTargetPattern = new($@"^\S+\s{Pattern.MultipleTargets}\s", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

    public SongRequestCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, Response* response, string? prefix, string alias)
    {
        ChatMessage = chatMessage;
        Response = response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        #region sr {users...} me

        Regex pattern = PatternCreator.Create(_alias, _prefix, $@"\s{Pattern.MultipleTargets}\sme");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            SpotifyUser? user = _twitchBot.SpotifyUsers[ChatMessage.Username];
            if (user is null)
            {
                Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, PredefinedMessages.YouArentRegisteredYouHaveToRegisterFirst);
                return;
            }

            SpotifyItem? item;
            try
            {
                Task<SpotifyItem?> task = SpotifyController.GetCurrentlyPlayingItem(user);
                task.Wait();
                item = task.Result;
            }
            catch (SpotifyException ex)
            {
                Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, ex.Message);
                return;
            }
            catch (AggregateException ex)
            {
                Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace);
                if (ex.InnerException is null)
                {
                    DbController.LogException(ex);
                    Response->Append(PredefinedMessages.ApiError);
                    return;
                }

                Response->Append(ex.InnerException.Message);
                return;
            }

            if (item is null or not SpotifyTrack)
            {
                Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, PredefinedMessages.YouArentListeningToATrack);
                return;
            }

            SpotifyUser[] targets = GetTargets();
            if (targets.Length == 0)
            {
                Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, PredefinedMessages.NoneOfTheGivenUsersAreRegistered);
                return;
            }

            SpotifyTrack track = (SpotifyTrack)item;
            Dictionary<SpotifyUser, string?> success = new();
            foreach (SpotifyUser target in targets)
            {
                try
                {
                    SpotifyController.AddToQueue(target, track.Uri).Wait();
                    success.Add(target, null);
                }
                catch (SpotifyException ex)
                {
                    success.Add(target, ex.Message);
                }
                catch (AggregateException ex)
                {
                    success.Add(target, ex.InnerException is null ? PredefinedMessages.ApiError : ex.InnerException.Message);
                }
            }

            CreateMultipleTargetResponse(success, track);
            return;
        }

        #endregion sr {user} me

        #region sr me {user}

        pattern = PatternCreator.Create(_alias, _prefix, @"\sme\s\w+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            SpotifyUser? user = _twitchBot.SpotifyUsers[ChatMessage.Username];
            if (user is null)
            {
                Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, PredefinedMessages.YouArentRegisteredYouHaveToRegisterFirst);
                return;
            }

            SpotifyUser? target = _twitchBot.SpotifyUsers[ChatMessage.LowerSplit[2]];
            if (target is null)
            {
                Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, ChatMessage.LowerSplit[2].Antiping(), " isn't registered yet, they have to register first");
                return;
            }

            SpotifyItem? item;
            try
            {
                Task<SpotifyItem?> task = SpotifyController.GetCurrentlyPlayingItem(target);
                task.Wait();
                item = task.Result;
            }
            catch (SpotifyException ex)
            {
                Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, ex.Message);
                return;
            }
            catch (AggregateException ex)
            {
                Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace);
                if (ex.InnerException is null)
                {
                    DbController.LogException(ex);
                    Response->Append(PredefinedMessages.ApiError);
                    return;
                }

                Response->Append(ex.InnerException.Message);
                return;
            }

            if (item is null or not SpotifyTrack)
            {
                Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, ChatMessage.LowerSplit[2].Antiping(), " isn't listening to a track");
                return;
            }

            try
            {
                SpotifyController.AddToQueue(user, item.Uri).Wait();
            }
            catch (SpotifyException ex)
            {
                Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, ex.Message);
                return;
            }
            catch (AggregateException ex)
            {
                Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace);
                if (ex.InnerException is null)
                {
                    DbController.LogException(ex);
                    Response->Append(PredefinedMessages.ApiError);
                    return;
                }

                Response->Append(ex.InnerException.Message);
                return;
            }

            SpotifyTrack track = (item as SpotifyTrack)!;
            Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, track.ToString(), " || ", track.IsLocal ? "local file" : track.Uri);
            Response->Append(" has been added to the queue of ", user.Username.Antiping());
            return;
        }

        #endregion

        #region sr {users...} {song}

        pattern = PatternCreator.Create(_alias, _prefix, $@"\s{Pattern.MultipleTargets}\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            SpotifyUser[] targets = GetTargets();
            if (targets.Length == 0)
            {
                Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, PredefinedMessages.NoneOfTheGivenUsersAreRegistered);
                return;
            }

            string song = _exceptTargetPattern.Replace(ChatMessage.Message, string.Empty);
            SpotifyTrack? track = null;
            try
            {
                Task<SpotifyTrack?> task = SpotifyController.SearchTrack(targets[0], song);
                task.Wait();
                track = task.Result;
            }
            catch (SpotifyException ex)
            {
                Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, ex.Message);
            }
            catch (AggregateException ex)
            {
                Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace);
                if (ex.InnerException is null)
                {
                    DbController.LogException(ex);
                    Response->Append(PredefinedMessages.ApiError);
                }
                else
                {
                    Response->Append(ex.InnerException.Message);
                }
            }

            if (track is null)
            {
                Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, PredefinedMessages.NoMatchingTrackCouldBeFound);
                return;
            }

            Dictionary<SpotifyUser, string?> success = new();
            foreach (SpotifyUser target in targets)
            {
                try
                {
                    SpotifyController.AddToQueue(target, track.Uri).Wait();
                    success.Add(target, null);
                }
                catch (SpotifyException ex)
                {
                    success.Add(target, ex.Message);
                }
                catch (AggregateException ex)
                {
                    success.Add(target, ex.InnerException is null ? PredefinedMessages.ApiError : ex.InnerException.Message);
                }
            }

            CreateMultipleTargetResponse(success, track);
            return;
        }

        #endregion sr {user} {song}

        #region sr me

        pattern = PatternCreator.Create(_alias, _prefix, @"\sme$");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            SpotifyUser? user = _twitchBot.SpotifyUsers[ChatMessage.Username];
            if (user is null)
            {
                Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, PredefinedMessages.YouArentRegisteredYouHaveToRegisterFirst);
                return;
            }

            SpotifyItem? item;
            try
            {
                Task<SpotifyItem?> task = SpotifyController.GetCurrentlyPlayingItem(user);
                task.Wait();
                item = task.Result;
            }
            catch (SpotifyException ex)
            {
                Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, ex.Message);
                return;
            }
            catch (AggregateException ex)
            {
                Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace);
                if (ex.InnerException is null)
                {
                    DbController.LogException(ex);
                    Response->Append(PredefinedMessages.ApiError);
                    return;
                }

                Response->Append(ex.InnerException.Message);
                return;
            }

            if (item is null or not SpotifyTrack)
            {
                Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, PredefinedMessages.YouArentListeningToATrack);
                return;
            }

            SpotifyUser? target = _twitchBot.SpotifyUsers[ChatMessage.Channel];
            if (target is null)
            {
                Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, ChatMessage.Channel.Antiping(), " isn't registered yet, they have to register first");
                return;
            }

            try
            {
                SpotifyController.AddToQueue(target, item.Uri).Wait();
            }
            catch (SpotifyException ex)
            {
                Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, ex.Message);
                return;
            }
            catch (AggregateException ex)
            {
                Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace);
                if (ex.InnerException is null)
                {
                    DbController.LogException(ex);
                    Response->Append(PredefinedMessages.ApiError);
                    return;
                }

                Response->Append(ex.InnerException.Message);
                return;
            }

            SpotifyTrack track = (item as SpotifyTrack)!;
            Response->Append(ChatMessage.Username, StringHelper.Whitespace, track.ToString(), " || ", track.IsLocal ? "local file" : track.Uri);
            Response->Append(" has been added to the queue of ", target.Username.Antiping());
            return;
        }

        #endregion sr me

        #region sr {song}

        pattern = PatternCreator.Create(_alias, _prefix, @"\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            SpotifyUser? target = _twitchBot.SpotifyUsers[ChatMessage.Channel];
            if (target is null)
            {
                Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, ChatMessage.Channel.Antiping(), " isn't registered yet, they have to register first");
                return;
            }

            try
            {
                Task<SpotifyTrack> task = SpotifyController.AddToQueue(target, ChatMessage.Message[(ChatMessage.Split[0].Length + 1)..]);
                task.Wait();
                SpotifyTrack track = task.Result;
                Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, track.ToString(), " || ", track.IsLocal ? "local file" : track.Uri);
                Response->Append(" has been added to the queue of ", target.Username.Antiping());
            }
            catch (SpotifyException ex)
            {
                Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, ex.Message);
            }
            catch (AggregateException ex)
            {
                Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace);
                if (ex.InnerException is null)
                {
                    DbController.LogException(ex);
                    Response->Append(PredefinedMessages.ApiError);
                    return;
                }

                Response->Append(ex.InnerException.Message);
            }
        }

        #endregion sr {song}
    }

    private SpotifyUser[] GetTargets()
    {
        Match match = Pattern.MultipleTargets.Match(ChatMessage.LowerSplit[1..^1].JoinToString(' '));
        string[] targets = match.Value.Split(',');
        TwitchBot twitchBot = _twitchBot;
        return targets.Select(t => t.TrimAll()).Distinct().Select(t => twitchBot.SpotifyUsers[t]).Where(t => t is not null).Take(5).ToArray()!;
    }

    private void CreateMultipleTargetResponse(Dictionary<SpotifyUser, string?> success, SpotifyTrack track)
    {
        string[] successUsers = success.Where(t => t.Value is null).Select(t => t.Key.Username.Antiping()).ToArray();
        string[] failedUsers = success.Where(t => t.Value is not null).Select(t => t.Value!).ToArray();

        if (successUsers.Length > 0)
        {
            Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, track.ToString(), " || ", track.IsLocal ? "local file" : track.Uri);
            Response->Append(" has been added to the queue", successUsers.Length > 1 ? "s of " : " of ", successUsers.JoinToString(PredefinedMessages.CommaSpace));
            if (failedUsers.Length == 0)
            {
                return;
            }

            Response->Append(". ", failedUsers.JoinToString(PredefinedMessages.CommaSpace));
        }
        else
        {
            if (failedUsers.Length == 1)
            {
                Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, failedUsers[0]);
            }
            else
            {
                Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, track.ToString(), " || ", track.IsLocal ? "local file" : track.Uri);
                Response->Append(" hasn't been added to any queue");
                if (failedUsers.Length == 0)
                {
                    return;
                }

                Response->Append(". ", failedUsers.JoinToString(PredefinedMessages.CommaSpace));
            }
        }
    }
}
