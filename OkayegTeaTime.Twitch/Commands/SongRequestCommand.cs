using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE.Memory;
using HLE.Strings;
using HLE.Twitch.Models;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Spotify;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;
using StringHelper = HLE.Strings.StringHelper;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.SongRequest)]
public readonly ref struct SongRequestCommand
{
    public ChatMessage ChatMessage { get; }

    private readonly TwitchBot _twitchBot;
    private readonly ref PoolBufferStringBuilder _response;
    private readonly ReadOnlySpan<char> _prefix;
    private readonly ReadOnlySpan<char> _alias;

    private static readonly Regex _exceptTargetPattern = new($@"^\S+\s{Pattern.MultipleTargets}\s", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1));

    public SongRequestCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref PoolBufferStringBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        ChatMessage = chatMessage;
        _response = ref response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        #region sr {users...} me

        Regex pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, $@"\s{Pattern.MultipleTargets}\sme");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            SpotifyUser? user = _twitchBot.SpotifyUsers[ChatMessage.Username];
            if (user is null)
            {
                _response.Append(ChatMessage.Username, ", ", Messages.YouArentRegisteredYouHaveToRegisterFirst);
                return;
            }

            SpotifyItem? item;
            try
            {
                Task<SpotifyItem?> task = SpotifyController.GetCurrentlyPlayingItemAsync(user);
                task.Wait();
                item = task.Result;
            }
            catch (SpotifyException ex)
            {
                _response.Append(ChatMessage.Username, ", ", ex.Message);
                return;
            }
            catch (AggregateException ex)
            {
                _response.Append(ChatMessage.Username, ", ");
                if (ex.InnerException is null)
                {
                    DbController.LogException(ex);
                    _response.Append(Messages.ApiError);
                    return;
                }

                _response.Append(ex.InnerException.Message);
                return;
            }

            if (item is null or not SpotifyTrack)
            {
                _response.Append(ChatMessage.Username, ", ", Messages.YouArentListeningToATrack);
                return;
            }

            SpotifyUser[] targets = GetTargets();
            if (targets.Length == 0)
            {
                _response.Append(ChatMessage.Username, ", ", Messages.NoneOfTheGivenUsersAreRegistered);
                return;
            }

            SpotifyTrack track = (SpotifyTrack)item;
            Dictionary<SpotifyUser, string?> success = new();
            foreach (SpotifyUser target in targets)
            {
                try
                {
                    SpotifyController.AddToQueueAsync(target, track.Uri).Wait();
                    success.Add(target, null);
                }
                catch (SpotifyException ex)
                {
                    success.Add(target, ex.Message);
                }
                catch (AggregateException ex)
                {
                    success.Add(target, ex.InnerException is null ? Messages.ApiError : ex.InnerException.Message);
                }
            }

            CreateMultipleTargetResponse(success, track);
            return;
        }

        #endregion sr {user} me

        #region sr me {user}

        pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\sme\s\w+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            SpotifyUser? user = _twitchBot.SpotifyUsers[ChatMessage.Username];
            if (user is null)
            {
                _response.Append(ChatMessage.Username, ", ", Messages.YouArentRegisteredYouHaveToRegisterFirst);
                return;
            }

            using ChatMessageExtension messageExtension = new(ChatMessage);
            string targetName = new(messageExtension.LowerSplit[2]);
            SpotifyUser? target = _twitchBot.SpotifyUsers[targetName];
            if (target is null)
            {
                _response.Append(ChatMessage.Username, ", ", targetName.Antiping(), " isn't registered yet, they have to register first");
                return;
            }

            SpotifyItem? item;
            try
            {
                Task<SpotifyItem?> task = SpotifyController.GetCurrentlyPlayingItemAsync(target);
                task.Wait();
                item = task.Result;
            }
            catch (SpotifyException ex)
            {
                _response.Append(ChatMessage.Username, ", ", ex.Message);
                return;
            }
            catch (AggregateException ex)
            {
                _response.Append(ChatMessage.Username, ", ");
                if (ex.InnerException is null)
                {
                    DbController.LogException(ex);
                    _response.Append(Messages.ApiError);
                    return;
                }

                _response.Append(ex.InnerException.Message);
                return;
            }

            if (item is null or not SpotifyTrack)
            {
                _response.Append(ChatMessage.Username, ", ", targetName.Antiping(), " isn't listening to a track");
                return;
            }

            try
            {
                SpotifyController.AddToQueueAsync(user, item.Uri).Wait();
            }
            catch (SpotifyException ex)
            {
                _response.Append(ChatMessage.Username, ", ", ex.Message);
                return;
            }
            catch (AggregateException ex)
            {
                _response.Append(ChatMessage.Username, ", ");
                if (ex.InnerException is null)
                {
                    DbController.LogException(ex);
                    _response.Append(Messages.ApiError);
                    return;
                }

                _response.Append(ex.InnerException.Message);
                return;
            }

            SpotifyTrack track = (item as SpotifyTrack)!;
            _response.Append(ChatMessage.Username, ", ", track.ToString(), " || ", track.IsLocal ? "local file" : track.Uri);
            _response.Append(" has been added to the queue of ", user.Username.Antiping());
            return;
        }

        #endregion

        #region sr {users...} {song}

        pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, $@"\s{Pattern.MultipleTargets}\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            SpotifyUser[] targets = GetTargets();
            if (targets.Length == 0)
            {
                _response.Append(ChatMessage.Username, ", ", Messages.NoneOfTheGivenUsersAreRegistered);
                return;
            }

            string song = _exceptTargetPattern.Replace(ChatMessage.Message, string.Empty);
            SpotifyTrack? track = null;
            try
            {
                Task<SpotifyTrack?> task = SpotifyController.SearchTrackAsync(targets[0], song);
                task.Wait();
                track = task.Result;
            }
            catch (SpotifyException ex)
            {
                _response.Append(ChatMessage.Username, ", ", ex.Message);
            }
            catch (AggregateException ex)
            {
                _response.Append(ChatMessage.Username, ", ");
                if (ex.InnerException is null)
                {
                    DbController.LogException(ex);
                    _response.Append(Messages.ApiError);
                }
                else
                {
                    _response.Append(ex.InnerException.Message);
                }
            }

            if (track is null)
            {
                _response.Append(ChatMessage.Username, ", ", Messages.NoMatchingTrackCouldBeFound);
                return;
            }

            Dictionary<SpotifyUser, string?> success = new();
            foreach (SpotifyUser target in targets)
            {
                try
                {
                    SpotifyController.AddToQueueAsync(target, track.Uri).Wait();
                    success.Add(target, null);
                }
                catch (SpotifyException ex)
                {
                    success.Add(target, ex.Message);
                }
                catch (AggregateException ex)
                {
                    success.Add(target, ex.InnerException is null ? Messages.ApiError : ex.InnerException.Message);
                }
            }

            CreateMultipleTargetResponse(success, track);
            return;
        }

        #endregion sr {user} {song}

        #region sr me

        pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\sme$");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            SpotifyUser? user = _twitchBot.SpotifyUsers[ChatMessage.Username];
            if (user is null)
            {
                _response.Append(ChatMessage.Username, ", ", Messages.YouArentRegisteredYouHaveToRegisterFirst);
                return;
            }

            SpotifyItem? item;
            try
            {
                Task<SpotifyItem?> task = SpotifyController.GetCurrentlyPlayingItemAsync(user);
                task.Wait();
                item = task.Result;
            }
            catch (SpotifyException ex)
            {
                _response.Append(ChatMessage.Username, ", ", ex.Message);
                return;
            }
            catch (AggregateException ex)
            {
                _response.Append(ChatMessage.Username, ", ");
                if (ex.InnerException is null)
                {
                    DbController.LogException(ex);
                    _response.Append(Messages.ApiError);
                    return;
                }

                _response.Append(ex.InnerException.Message);
                return;
            }

            if (item is null or not SpotifyTrack)
            {
                _response.Append(ChatMessage.Username, ", ", Messages.YouArentListeningToATrack);
                return;
            }

            SpotifyUser? target = _twitchBot.SpotifyUsers[ChatMessage.Channel];
            if (target is null)
            {
                _response.Append(ChatMessage.Username, ", ", ChatMessage.Channel.Antiping(), " isn't registered yet, they have to register first");
                return;
            }

            try
            {
                SpotifyController.AddToQueueAsync(target, item.Uri).Wait();
            }
            catch (SpotifyException ex)
            {
                _response.Append(ChatMessage.Username, ", ", ex.Message);
                return;
            }
            catch (AggregateException ex)
            {
                _response.Append(ChatMessage.Username, ", ");
                if (ex.InnerException is null)
                {
                    DbController.LogException(ex);
                    _response.Append(Messages.ApiError);
                    return;
                }

                _response.Append(ex.InnerException.Message);
                return;
            }

            SpotifyTrack track = (item as SpotifyTrack)!;
            _response.Append(ChatMessage.Username, " ", track.ToString(), " || ", track.IsLocal ? "local file" : track.Uri);
            _response.Append(" has been added to the queue of ", target.Username.Antiping());
            return;
        }

        #endregion sr me

        #region sr {song}

        pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            SpotifyUser? target = _twitchBot.SpotifyUsers[ChatMessage.Channel];
            if (target is null)
            {
                _response.Append(ChatMessage.Username, ", ", ChatMessage.Channel.Antiping(), " isn't registered yet, they have to register first");
                return;
            }

            try
            {
                using ChatMessageExtension messageExtension = new(ChatMessage);
                Task<SpotifyTrack> task = SpotifyController.AddToQueueAsync(target, ChatMessage.Message[(messageExtension.Split[0].Length + 1)..]);
                task.Wait();
                SpotifyTrack track = task.Result;
                _response.Append(ChatMessage.Username, ", ", track.ToString(), " || ", track.IsLocal ? "local file" : track.Uri);
                _response.Append(" has been added to the queue of ", target.Username.Antiping());
            }
            catch (SpotifyException ex)
            {
                _response.Append(ChatMessage.Username, ", ", ex.Message);
            }
            catch (AggregateException ex)
            {
                _response.Append(ChatMessage.Username, ", ");
                if (ex.InnerException is null)
                {
                    DbController.LogException(ex);
                    _response.Append(Messages.ApiError);
                    return;
                }

                _response.Append(ex.InnerException.Message);
            }
        }

        #endregion sr {song}
    }

    private SpotifyUser[] GetTargets()
    {
        using ChatMessageExtension messageExtension = new(ChatMessage);
        using RentedArray<ReadOnlyMemory<char>> splits = messageExtension.LowerSplit.GetSplits();
        Span<char> targetBuffer = stackalloc char[500];
        int targetBufferLength = StringHelper.Join(splits[1..^1], ' ', targetBuffer);
        string targetString = new(targetBuffer[..targetBufferLength]);

        Match match = Pattern.MultipleTargets.Match(targetString);
        string[] targets = match.Value.Split(',');
        TwitchBot twitchBot = _twitchBot;
        return targets.Select(t => t.TrimAll()).Distinct().Select(t => twitchBot.SpotifyUsers[t]).Where(t => t is not null).Take(5).ToArray()!;
    }

    private void CreateMultipleTargetResponse(Dictionary<SpotifyUser, string?> success, SpotifyTrack track)
    {
        string[] successUsers = success.Where(t => t.Value is null).Select(t => t.Key.Username.Antiping()).ToArray();
        string[] failedUsers = success.Where(t => t.Value is not null).Select(t => t.Value!).ToArray();

        Span<char> joinBuffer = stackalloc char[500];
        int bufferLength;
        if (successUsers.Length > 0)
        {
            bufferLength = StringHelper.Join(successUsers, ", ", joinBuffer);
            _response.Append(ChatMessage.Username, ", ", track.ToString(), " || ", track.IsLocal ? "local file" : track.Uri);
            _response.Append(" has been added to the queue", successUsers.Length > 1 ? "s of " : " of ", joinBuffer[..bufferLength]);
            if (failedUsers.Length == 0)
            {
                return;
            }

            bufferLength = StringHelper.Join(failedUsers, ", ", joinBuffer);
            _response.Append(". ", joinBuffer[..bufferLength]);
        }
        else
        {
            if (failedUsers.Length == 1)
            {
                _response.Append(ChatMessage.Username, ", ", failedUsers[0]);
            }
            else
            {
                _response.Append(ChatMessage.Username, ", ", track.ToString(), " || ", track.IsLocal ? "local file" : track.Uri);
                _response.Append(" hasn't been added to any queue");
                if (failedUsers.Length == 0)
                {
                    return;
                }

                bufferLength = StringHelper.Join(failedUsers, ", ", joinBuffer);
                _response.Append(". ", joinBuffer[..bufferLength]);
            }
        }
    }
}
