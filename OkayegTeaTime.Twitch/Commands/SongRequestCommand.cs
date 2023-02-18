using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE;
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

    public StringBuilder* Response { get; }

    private readonly TwitchBot _twitchBot;
    private readonly string? _prefix;
    private readonly string _alias;

    private static readonly Regex _exceptTargetPattern = new($@"^\S+\s{Pattern.MultipleTargets}\s", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1));

    public SongRequestCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
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

        Regex pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, $@"\s{Pattern.MultipleTargets}\sme");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            SpotifyUser? user = _twitchBot.SpotifyUsers[ChatMessage.Username];
            if (user is null)
            {
                Response->Append(ChatMessage.Username, Messages.CommaSpace, Messages.YouArentRegisteredYouHaveToRegisterFirst);
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
                Response->Append(ChatMessage.Username, Messages.CommaSpace, ex.Message);
                return;
            }
            catch (AggregateException ex)
            {
                Response->Append(ChatMessage.Username, Messages.CommaSpace);
                if (ex.InnerException is null)
                {
                    DbController.LogException(ex);
                    Response->Append(Messages.ApiError);
                    return;
                }

                Response->Append(ex.InnerException.Message);
                return;
            }

            if (item is null or not SpotifyTrack)
            {
                Response->Append(ChatMessage.Username, Messages.CommaSpace, Messages.YouArentListeningToATrack);
                return;
            }

            SpotifyUser[] targets = GetTargets();
            if (targets.Length == 0)
            {
                Response->Append(ChatMessage.Username, Messages.CommaSpace, Messages.NoneOfTheGivenUsersAreRegistered);
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
                Response->Append(ChatMessage.Username, Messages.CommaSpace, Messages.YouArentRegisteredYouHaveToRegisterFirst);
                return;
            }

            SpotifyUser? target = _twitchBot.SpotifyUsers[ChatMessage.LowerSplit[2]];
            if (target is null)
            {
                Response->Append(ChatMessage.Username, Messages.CommaSpace, ChatMessage.LowerSplit[2].Antiping(), " isn't registered yet, they have to register first");
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
                Response->Append(ChatMessage.Username, Messages.CommaSpace, ex.Message);
                return;
            }
            catch (AggregateException ex)
            {
                Response->Append(ChatMessage.Username, Messages.CommaSpace);
                if (ex.InnerException is null)
                {
                    DbController.LogException(ex);
                    Response->Append(Messages.ApiError);
                    return;
                }

                Response->Append(ex.InnerException.Message);
                return;
            }

            if (item is null or not SpotifyTrack)
            {
                Response->Append(ChatMessage.Username, Messages.CommaSpace, ChatMessage.LowerSplit[2].Antiping(), " isn't listening to a track");
                return;
            }

            try
            {
                SpotifyController.AddToQueueAsync(user, item.Uri).Wait();
            }
            catch (SpotifyException ex)
            {
                Response->Append(ChatMessage.Username, Messages.CommaSpace, ex.Message);
                return;
            }
            catch (AggregateException ex)
            {
                Response->Append(ChatMessage.Username, Messages.CommaSpace);
                if (ex.InnerException is null)
                {
                    DbController.LogException(ex);
                    Response->Append(Messages.ApiError);
                    return;
                }

                Response->Append(ex.InnerException.Message);
                return;
            }

            SpotifyTrack track = (item as SpotifyTrack)!;
            Response->Append(ChatMessage.Username, Messages.CommaSpace, track.ToString(), " || ", track.IsLocal ? "local file" : track.Uri);
            Response->Append(" has been added to the queue of ", user.Username.Antiping());
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
                Response->Append(ChatMessage.Username, Messages.CommaSpace, Messages.NoneOfTheGivenUsersAreRegistered);
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
                Response->Append(ChatMessage.Username, Messages.CommaSpace, ex.Message);
            }
            catch (AggregateException ex)
            {
                Response->Append(ChatMessage.Username, Messages.CommaSpace);
                if (ex.InnerException is null)
                {
                    DbController.LogException(ex);
                    Response->Append(Messages.ApiError);
                }
                else
                {
                    Response->Append(ex.InnerException.Message);
                }
            }

            if (track is null)
            {
                Response->Append(ChatMessage.Username, Messages.CommaSpace, Messages.NoMatchingTrackCouldBeFound);
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
                Response->Append(ChatMessage.Username, Messages.CommaSpace, Messages.YouArentRegisteredYouHaveToRegisterFirst);
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
                Response->Append(ChatMessage.Username, Messages.CommaSpace, ex.Message);
                return;
            }
            catch (AggregateException ex)
            {
                Response->Append(ChatMessage.Username, Messages.CommaSpace);
                if (ex.InnerException is null)
                {
                    DbController.LogException(ex);
                    Response->Append(Messages.ApiError);
                    return;
                }

                Response->Append(ex.InnerException.Message);
                return;
            }

            if (item is null or not SpotifyTrack)
            {
                Response->Append(ChatMessage.Username, Messages.CommaSpace, Messages.YouArentListeningToATrack);
                return;
            }

            SpotifyUser? target = _twitchBot.SpotifyUsers[ChatMessage.Channel];
            if (target is null)
            {
                Response->Append(ChatMessage.Username, Messages.CommaSpace, ChatMessage.Channel.Antiping(), " isn't registered yet, they have to register first");
                return;
            }

            try
            {
                SpotifyController.AddToQueueAsync(target, item.Uri).Wait();
            }
            catch (SpotifyException ex)
            {
                Response->Append(ChatMessage.Username, Messages.CommaSpace, ex.Message);
                return;
            }
            catch (AggregateException ex)
            {
                Response->Append(ChatMessage.Username, Messages.CommaSpace);
                if (ex.InnerException is null)
                {
                    DbController.LogException(ex);
                    Response->Append(Messages.ApiError);
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

        pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            SpotifyUser? target = _twitchBot.SpotifyUsers[ChatMessage.Channel];
            if (target is null)
            {
                Response->Append(ChatMessage.Username, Messages.CommaSpace, ChatMessage.Channel.Antiping(), " isn't registered yet, they have to register first");
                return;
            }

            try
            {
                Task<SpotifyTrack> task = SpotifyController.AddToQueueAsync(target, ChatMessage.Message[(ChatMessage.Split[0].Length + 1)..]);
                task.Wait();
                SpotifyTrack track = task.Result;
                Response->Append(ChatMessage.Username, Messages.CommaSpace, track.ToString(), " || ", track.IsLocal ? "local file" : track.Uri);
                Response->Append(" has been added to the queue of ", target.Username.Antiping());
            }
            catch (SpotifyException ex)
            {
                Response->Append(ChatMessage.Username, Messages.CommaSpace, ex.Message);
            }
            catch (AggregateException ex)
            {
                Response->Append(ChatMessage.Username, Messages.CommaSpace);
                if (ex.InnerException is null)
                {
                    DbController.LogException(ex);
                    Response->Append(Messages.ApiError);
                    return;
                }

                Response->Append(ex.InnerException.Message);
            }
        }

        #endregion sr {song}
    }

    private SpotifyUser[] GetTargets()
    {
        Match match = Pattern.MultipleTargets.Match(string.Join(' ', ChatMessage.LowerSplit, 1, ChatMessage.LowerSplit.Length - 2));
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
            bufferLength = StringHelper.Join(successUsers, Messages.CommaSpace, joinBuffer);
            Response->Append(ChatMessage.Username, Messages.CommaSpace, track.ToString(), " || ", track.IsLocal ? "local file" : track.Uri);
            Response->Append(" has been added to the queue", successUsers.Length > 1 ? "s of " : " of ", joinBuffer[..bufferLength]);
            if (failedUsers.Length == 0)
            {
                return;
            }

            bufferLength = StringHelper.Join(failedUsers, Messages.CommaSpace, joinBuffer);
            Response->Append(". ", joinBuffer[..bufferLength]);
        }
        else
        {
            if (failedUsers.Length == 1)
            {
                Response->Append(ChatMessage.Username, Messages.CommaSpace, failedUsers[0]);
            }
            else
            {
                Response->Append(ChatMessage.Username, Messages.CommaSpace, track.ToString(), " || ", track.IsLocal ? "local file" : track.Uri);
                Response->Append(" hasn't been added to any queue");
                if (failedUsers.Length == 0)
                {
                    return;
                }

                bufferLength = StringHelper.Join(failedUsers, Messages.CommaSpace, joinBuffer);
                Response->Append(". ", joinBuffer[..bufferLength]);
            }
        }
    }
}
