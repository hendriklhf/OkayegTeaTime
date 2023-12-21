using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE.Strings;
using HLE.Twitch.Models;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Spotify;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.SongRequest, typeof(SongRequestCommand))]
public readonly struct SongRequestCommand(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    : IChatCommand<SongRequestCommand>
{
    public PooledStringBuilder Response { get; } = new(GlobalSettings.MaxMessageLength);

    public IChatMessage ChatMessage { get; } = chatMessage;

    private readonly TwitchBot _twitchBot = twitchBot;
    private readonly ReadOnlyMemory<char> _prefix = prefix;
    private readonly ReadOnlyMemory<char> _alias = alias;

    private static readonly Regex s_exceptTargetPattern = new($@"^\S+\s{Pattern.MultipleTargets}\s", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1));

    public static void Create(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out SongRequestCommand command)
        => command = new(twitchBot, chatMessage, prefix, alias);

    public async ValueTask Handle()
    {
        if (GlobalSettings.Settings.Spotify is null)
        {
            Response.Append(ChatMessage.Username, ", ", Messages.TheCommandHasNotBeenConfiguredByTheBotOwner);
            return;
        }

        Regex pattern = _twitchBot.MessageRegexCreator.Create(_alias.Span, _prefix.Span, $@"\s{Pattern.MultipleTargets}\sme");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            await RequestSongOfSenderToMultipleUsersAsync();
            return;
        }

        pattern = _twitchBot.MessageRegexCreator.Create(_alias.Span, _prefix.Span, @"\sme\s\w+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            await RequestSongOfTargetToSenderAsync();
            return;
        }

        pattern = _twitchBot.MessageRegexCreator.Create(_alias.Span, _prefix.Span, $@"\s{Pattern.MultipleTargets}\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            await RequestSongBySearchTermsToMultipleUsersAsync();
            return;
        }

        pattern = _twitchBot.MessageRegexCreator.Create(_alias.Span, _prefix.Span, @"\sme$");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            await RequestSongOfSenderToStreamerAsync();
            return;
        }

        pattern = _twitchBot.MessageRegexCreator.Create(_alias.Span, _prefix.Span, @"\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            await RequestSongBySearchTermToStreamerAsync();
        }
    }

    private async Task RequestSongBySearchTermToStreamerAsync()
    {
        SpotifyUser? target = _twitchBot.SpotifyUsers[ChatMessage.Channel];
        if (target is null)
        {
            Response.Append(ChatMessage.Username, ", ", ChatMessage.Channel.Antiping(), " isn't registered yet, they have to register first");
            return;
        }

        try
        {
            using ChatMessageExtension messageExtension = new(ChatMessage);
            string song = ChatMessage.Message[(messageExtension.Split[0].Length + 1)..];
            SpotifyTrack track = await SpotifyController.AddToQueueAsync(target, song);
            Response.Append(ChatMessage.Username, ", ", track.ToString(), " || ", track.IsLocal ? "local file" : track.Uri);
            Response.Append(" has been added to the queue of ", target.Username.Antiping());
        }
        catch (SpotifyException ex)
        {
            Response.Append(ChatMessage.Username, ", ", ex.Message);
        }
        catch (AggregateException ex)
        {
            Response.Append(ChatMessage.Username, ", ");
            if (ex.InnerException is null)
            {
                await DbController.LogExceptionAsync(ex);
                Response.Append(Messages.ApiError);
                return;
            }

            Response.Append(ex.InnerException.Message);
        }
    }

    private async Task RequestSongOfSenderToStreamerAsync()
    {
        SpotifyUser? user = _twitchBot.SpotifyUsers[ChatMessage.Username];
        if (user is null)
        {
            Response.Append(ChatMessage.Username, ", ", Messages.YouArentRegisteredYouHaveToRegisterFirst);
            return;
        }

        SpotifyItem? item;
        try
        {
            item = await SpotifyController.GetCurrentlyPlayingItemAsync(user);
        }
        catch (SpotifyException ex)
        {
            Response.Append(ChatMessage.Username, ", ", ex.Message);
            return;
        }
        catch (AggregateException ex)
        {
            Response.Append(ChatMessage.Username, ", ");
            if (ex.InnerException is null)
            {
                await DbController.LogExceptionAsync(ex);
                Response.Append(Messages.ApiError);
                return;
            }

            Response.Append(ex.InnerException.Message);
            return;
        }

        if (item is null or not SpotifyTrack)
        {
            Response.Append(ChatMessage.Username, ", ", Messages.YouArentListeningToATrack);
            return;
        }

        SpotifyUser? target = _twitchBot.SpotifyUsers[ChatMessage.Channel];
        if (target is null)
        {
            Response.Append(ChatMessage.Username, ", ", ChatMessage.Channel.Antiping(), " isn't registered yet, they have to register first");
            return;
        }

        try
        {
            await SpotifyController.AddToQueueAsync(target, item.Uri);
        }
        catch (SpotifyException ex)
        {
            Response.Append(ChatMessage.Username, ", ", ex.Message);
            return;
        }
        catch (AggregateException ex)
        {
            Response.Append(ChatMessage.Username, ", ");
            if (ex.InnerException is null)
            {
                await DbController.LogExceptionAsync(ex);
                Response.Append(Messages.ApiError);
                return;
            }

            Response.Append(ex.InnerException.Message);
            return;
        }

        SpotifyTrack track = (item as SpotifyTrack)!;
        Response.Append(ChatMessage.Username, " ", track.ToString(), " || ", track.IsLocal ? "local file" : track.Uri);
        Response.Append(" has been added to the queue of ", target.Username.Antiping());
    }

    private async Task RequestSongBySearchTermsToMultipleUsersAsync()
    {
        SpotifyUser[] targets = GetTargets();
        if (targets.Length == 0)
        {
            Response.Append(ChatMessage.Username, ", ", Messages.NoneOfTheGivenUsersAreRegistered);
            return;
        }

        string song = s_exceptTargetPattern.Replace(ChatMessage.Message, string.Empty);
        SpotifyTrack? track = null;
        try
        {
            track = await SpotifyController.SearchTrackAsync(targets[0], song);
        }
        catch (SpotifyException ex)
        {
            Response.Append(ChatMessage.Username, ", ", ex.Message);
        }
        catch (AggregateException ex)
        {
            Response.Append(ChatMessage.Username, ", ");
            if (ex.InnerException is null)
            {
                await DbController.LogExceptionAsync(ex);
                Response.Append(Messages.ApiError);
            }
            else
            {
                Response.Append(ex.InnerException.Message);
            }
        }

        if (track is null)
        {
            Response.Append(ChatMessage.Username, ", ", Messages.NoMatchingTrackCouldBeFound);
            return;
        }

        Dictionary<SpotifyUser, string?> success = [];
        foreach (SpotifyUser target in targets)
        {
            try
            {
                await SpotifyController.AddToQueueAsync(target, track.Uri);
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
    }

    private async Task RequestSongOfTargetToSenderAsync()
    {
        SpotifyUser? user = _twitchBot.SpotifyUsers[ChatMessage.Username];
        if (user is null)
        {
            Response.Append(ChatMessage.Username, ", ", Messages.YouArentRegisteredYouHaveToRegisterFirst);
            return;
        }

        using ChatMessageExtension messageExtension = new(ChatMessage);
        string targetName = new(messageExtension.LowerSplit[2].Span);
        SpotifyUser? target = _twitchBot.SpotifyUsers[targetName];
        if (target is null)
        {
            Response.Append(ChatMessage.Username, ", ", targetName.Antiping(), " isn't registered yet, they have to register first");
            return;
        }

        SpotifyItem? item;
        try
        {
            item = await SpotifyController.GetCurrentlyPlayingItemAsync(target);
        }
        catch (SpotifyException ex)
        {
            Response.Append(ChatMessage.Username, ", ", ex.Message);
            return;
        }
        catch (AggregateException ex)
        {
            Response.Append(ChatMessage.Username, ", ");
            if (ex.InnerException is null)
            {
                await DbController.LogExceptionAsync(ex);
                Response.Append(Messages.ApiError);
                return;
            }

            Response.Append(ex.InnerException.Message);
            return;
        }

        if (item is null or not SpotifyTrack)
        {
            Response.Append(ChatMessage.Username, ", ", targetName.Antiping(), " isn't listening to a track");
            return;
        }

        try
        {
            await SpotifyController.AddToQueueAsync(user, item.Uri);
        }
        catch (SpotifyException ex)
        {
            Response.Append(ChatMessage.Username, ", ", ex.Message);
            return;
        }
        catch (AggregateException ex)
        {
            Response.Append(ChatMessage.Username, ", ");
            if (ex.InnerException is null)
            {
                await DbController.LogExceptionAsync(ex);
                Response.Append(Messages.ApiError);
                return;
            }

            Response.Append(ex.InnerException.Message);
            return;
        }

        SpotifyTrack track = (SpotifyTrack)item;
        Response.Append(ChatMessage.Username, ", ", track.ToString(), " || ", track.IsLocal ? "local file" : track.Uri);
        Response.Append(" has been added to the queue of ", user.Username.Antiping());
    }

    private async ValueTask RequestSongOfSenderToMultipleUsersAsync()
    {
        SpotifyUser? user = _twitchBot.SpotifyUsers[ChatMessage.Username];
        if (user is null)
        {
            Response.Append(ChatMessage.Username, ", ", Messages.YouArentRegisteredYouHaveToRegisterFirst);
            return;
        }

        SpotifyItem? item;
        try
        {
            item = await SpotifyController.GetCurrentlyPlayingItemAsync(user);
        }
        catch (SpotifyException ex)
        {
            Response.Append(ChatMessage.Username, ", ", ex.Message);
            return;
        }
        catch (AggregateException ex)
        {
            Response.Append(ChatMessage.Username, ", ");
            if (ex.InnerException is null)
            {
                await DbController.LogExceptionAsync(ex);
                Response.Append(Messages.ApiError);
                return;
            }

            Response.Append(ex.InnerException.Message);
            return;
        }

        if (item is null or not SpotifyTrack)
        {
            Response.Append(ChatMessage.Username, ", ", Messages.YouArentListeningToATrack);
            return;
        }

        SpotifyUser[] targets = GetTargets();
        if (targets.Length == 0)
        {
            Response.Append(ChatMessage.Username, ", ", Messages.NoneOfTheGivenUsersAreRegistered);
            return;
        }

        SpotifyTrack track = (SpotifyTrack)item;
        Dictionary<SpotifyUser, string?> success = [];
        foreach (SpotifyUser target in targets)
        {
            try
            {
                await SpotifyController.AddToQueueAsync(target, track.Uri);
                success.Add(target, null);
            }
            catch (SpotifyException ex)
            {
                success.Add(target, ex.Message);
            }
        }

        CreateMultipleTargetResponse(success, track);
    }

    private SpotifyUser[] GetTargets()
    {
        using ChatMessageExtension messageExtension = new(ChatMessage);
        ReadOnlySpan<ReadOnlyMemory<char>> splits = messageExtension.LowerSplit.AsSpan();
        Span<char> targetBuffer = stackalloc char[512];
        int targetBufferLength = StringHelpers.Join(' ', splits[1..^1], targetBuffer);
        string targetString = new(targetBuffer[..targetBufferLength]);

        Match match = Pattern.MultipleTargets.Match(targetString);
        string[] targets = match.Value.Split(',');
        TwitchBot twitchBot = _twitchBot;
        return targets.Select(static t => t.TrimAll()).Distinct().Select(t => twitchBot.SpotifyUsers[t]).Where(static t => t is not null).Take(5).ToArray()!;
    }

    private void CreateMultipleTargetResponse(Dictionary<SpotifyUser, string?> success, SpotifyTrack track)
    {
        string[] successUsers = success.Where(static t => t.Value is null).Select(static t => t.Key.Username.Antiping()).ToArray();
        string[] failedUsers = success.Where(static t => t.Value is not null).Select(static t => t.Value!).ToArray();

        Span<char> joinBuffer = stackalloc char[512];
        int bufferLength;
        if (successUsers.Length != 0)
        {
            bufferLength = StringHelpers.Join(", ", successUsers, joinBuffer);
            Response.Append(ChatMessage.Username, ", ", track.ToString(), " || ", track.IsLocal ? "local file" : track.Uri);
            Response.Append(" has been added to the queue", successUsers.Length > 1 ? "s of " : " of ", joinBuffer[..bufferLength]);
            if (failedUsers.Length == 0)
            {
                return;
            }

            bufferLength = StringHelpers.Join(", ", failedUsers, joinBuffer);
            Response.Append(". ", joinBuffer[..bufferLength]);
        }
        else
        {
            if (failedUsers.Length == 1)
            {
                Response.Append(ChatMessage.Username, ", ", failedUsers[0]);
            }
            else
            {
                Response.Append(ChatMessage.Username, ", ", track.ToString(), " || ", track.IsLocal ? "local file" : track.Uri);
                Response.Append(" hasn't been added to any queue");
                if (failedUsers.Length == 0)
                {
                    return;
                }

                bufferLength = StringHelpers.Join(", ", failedUsers, joinBuffer);
                Response.Append(". ", joinBuffer[..bufferLength]);
            }
        }
    }

    public void Dispose() => Response.Dispose();

    public bool Equals(SongRequestCommand other) =>
        _twitchBot.Equals(other._twitchBot) && _prefix.Equals(other._prefix) && _alias.Equals(other._alias) &&
        Response.Equals(other.Response) && ChatMessage.Equals(other.ChatMessage);

    public override bool Equals(object? obj) => obj is SongRequestCommand other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(_twitchBot, _prefix, _alias, Response, ChatMessage);

    public static bool operator ==(SongRequestCommand left, SongRequestCommand right) => left.Equals(right);

    public static bool operator !=(SongRequestCommand left, SongRequestCommand right) => !left.Equals(right);
}
