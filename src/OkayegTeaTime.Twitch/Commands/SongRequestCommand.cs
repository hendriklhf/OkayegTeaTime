using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE.Text;
using HLE.Twitch.Tmi.Models;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Configuration;
using OkayegTeaTime.Spotify;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand<SongRequestCommand>(CommandType.SongRequest)]
public readonly struct SongRequestCommand(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    : IChatCommand<SongRequestCommand>
{
    public PooledStringBuilder Response { get; } = new(GlobalSettings.MaxMessageLength);

    public ChatMessage ChatMessage { get; } = chatMessage;

    private readonly TwitchBot _twitchBot = twitchBot;
    private readonly ReadOnlyMemory<char> _prefix = prefix;
    private readonly ReadOnlyMemory<char> _alias = alias;

    private static readonly Regex s_exceptTargetPattern = new($@"^\S+\s{Pattern.MultipleTargets}\s", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1));

    private const string LocalFileDescription = "local file";

    public static void Create(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out SongRequestCommand command)
        => command = new(twitchBot, chatMessage, prefix, alias);

    public async ValueTask HandleAsync()
    {
        if (GlobalSettings.Settings.Spotify is null)
        {
            Response.Append($"{ChatMessage.Username}, {Texts.TheCommandHasNotBeenConfiguredByTheBotOwner}");
            return;
        }

        Regex pattern = _twitchBot.MessageRegexCreator.Create(_alias.Span, _prefix.Span, $@"\s{Pattern.MultipleTargets}\sme");
        if (pattern.IsMatch(ChatMessage.Message.AsSpan()))
        {
            await RequestSongOfSenderToMultipleUsersAsync();
            return;
        }

        pattern = _twitchBot.MessageRegexCreator.Create(_alias.Span, _prefix.Span, @"\sme\s\w+");
        if (pattern.IsMatch(ChatMessage.Message.AsSpan()))
        {
            await RequestSongOfTargetToSenderAsync();
            return;
        }

        pattern = _twitchBot.MessageRegexCreator.Create(_alias.Span, _prefix.Span, $@"\s{Pattern.MultipleTargets}\s\S+");
        if (pattern.IsMatch(ChatMessage.Message.AsSpan()))
        {
            await RequestSongBySearchTermsToMultipleUsersAsync();
            return;
        }

        pattern = _twitchBot.MessageRegexCreator.Create(_alias.Span, _prefix.Span, @"\sme$");
        if (pattern.IsMatch(ChatMessage.Message.AsSpan()))
        {
            await RequestSongOfSenderToStreamerAsync();
            return;
        }

        pattern = _twitchBot.MessageRegexCreator.Create(_alias.Span, _prefix.Span, @"\s\S+");
        if (pattern.IsMatch(ChatMessage.Message.AsSpan()))
        {
            await RequestSongBySearchTermToStreamerAsync();
        }
    }

    private async Task RequestSongBySearchTermToStreamerAsync()
    {
        SpotifyUser? target = _twitchBot.SpotifyUsers[ChatMessage.Channel];
        if (target is null)
        {
            Response.Append($"{ChatMessage.Username}, {ChatMessage.Channel.Antiping()} isn't registered yet, they have to register first");
            return;
        }

        try
        {
            using ChatMessageExtension messageExtension = new(ChatMessage);
            // TODO: fix ToString
            string song = ChatMessage.Message.ToString()[(messageExtension.Split[0].Length + 1)..];
            SpotifyTrack track = await SpotifyController.AddToQueueAsync(target, song);
            Response.Append($"{ChatMessage.Username}, {track} || {(track.IsLocal ? LocalFileDescription : track.Uri)}");
            Response.Append($" has been added to the queue of {target.Username.Antiping()}");
        }
        catch (SpotifyException ex)
        {
            Response.Append($"{ChatMessage.Username}, {ex.Message}");
        }
        catch (AggregateException ex)
        {
            Response.Append(ChatMessage.Username, ", ");
            if (ex.InnerException is null)
            {
                await DbController.LogExceptionAsync(ex);
                Response.Append(Texts.ApiError);
                return;
            }

            Response.Append(ex.InnerException.Message);
        }
    }

    private async Task RequestSongOfSenderToStreamerAsync()
    {
        SpotifyUser? user = _twitchBot.SpotifyUsers[ChatMessage.Username.ToString()];
        if (user is null)
        {
            Response.Append($"{ChatMessage.Username}, {Texts.YouArentRegisteredYouHaveToRegisterFirst}");
            return;
        }

        try
        {
            SpotifyItem? item = await SpotifyController.GetCurrentlyPlayingItemAsync(user);
            if (item is not SpotifyTrack track)
            {
                Response.Append($"{ChatMessage.Username}, {Texts.YouArentListeningToATrack}");
                return;
            }

            SpotifyUser? target = _twitchBot.SpotifyUsers[ChatMessage.Channel];
            if (target is null)
            {
                Response.Append($"{ChatMessage.Username}, {ChatMessage.Channel.Antiping()} isn't registered yet, they have to register first");
                return;
            }

            await SpotifyController.AddToQueueAsync(target, track.Uri);

            Response.Append($"{ChatMessage.Username}, {track} || {(track.IsLocal ? LocalFileDescription : track.Uri)}");
            Response.Append($" has been added to the queue of {target.Username.Antiping()}");
        }
        catch (SpotifyException ex)
        {
            Response.Append($"{ChatMessage.Username}, {ex.Message}");
        }
    }

    private async Task RequestSongBySearchTermsToMultipleUsersAsync()
    {
        SpotifyUser[] targets = GetTargets();
        if (targets.Length == 0)
        {
            Response.Append($"{ChatMessage.Username}, {Texts.NoneOfTheGivenUsersAreRegistered}");
            return;
        }

        string song = s_exceptTargetPattern.Replace(ChatMessage.Message.ToString(), string.Empty);
        SpotifyTrack? track = null;
        try
        {
            track = await SpotifyController.SearchTrackAsync(targets[0], song);
        }
        catch (SpotifyException ex)
        {
            Response.Append($"{ChatMessage.Username}, {ex.Message}");
        }
        catch (AggregateException ex)
        {
            Response.Append(ChatMessage.Username, ", ");
            if (ex.InnerException is null)
            {
                await DbController.LogExceptionAsync(ex);
                Response.Append(Texts.ApiError);
            }
            else
            {
                Response.Append(ex.InnerException.Message);
            }
        }

        if (track is null)
        {
            Response.Append($"{ChatMessage.Username}, {Texts.NoMatchingTrackCouldBeFound}");
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
                success.Add(target, ex.InnerException is null ? Texts.ApiError : ex.InnerException.Message);
            }
        }

        CreateMultipleTargetResponse(success, track);
    }

    private async Task RequestSongOfTargetToSenderAsync()
    {
        SpotifyUser? user = _twitchBot.SpotifyUsers[ChatMessage.Username.ToString()];
        if (user is null)
        {
            Response.Append($"{ChatMessage.Username}, {Texts.YouArentRegisteredYouHaveToRegisterFirst}");
            return;
        }

        using ChatMessageExtension messageExtension = new(ChatMessage);
        string targetName = new(messageExtension.LowerSplit[2].Span);
        SpotifyUser? target = _twitchBot.SpotifyUsers[targetName];
        if (target is null)
        {
            Response.Append($"{ChatMessage.Username}, {targetName.Antiping()} isn't registered yet, they have to register first");
            return;
        }

        SpotifyItem? item;
        try
        {
            item = await SpotifyController.GetCurrentlyPlayingItemAsync(target);
            if (item is not SpotifyTrack)
            {
                Response.Append($"{ChatMessage.Username}, {targetName.Antiping()} isn't listening to a track");
                return;
            }

            await SpotifyController.AddToQueueAsync(user, item.Uri);
        }
        catch (SpotifyException ex)
        {
            Response.Append($"{ChatMessage.Username}, {ex.Message}");
            return;
        }

        SpotifyTrack track = (SpotifyTrack)item;
        Response.Append($"{ChatMessage.Username}, {track} || {(track.IsLocal ? LocalFileDescription : track.Uri)}");
        Response.Append(" has been added to the queue of ", user.Username.Antiping());
    }

    private async ValueTask RequestSongOfSenderToMultipleUsersAsync()
    {
        SpotifyUser? user = _twitchBot.SpotifyUsers[ChatMessage.Username.ToString()];
        if (user is null)
        {
            Response.Append($"{ChatMessage.Username}, {Texts.YouArentRegisteredYouHaveToRegisterFirst}");
            return;
        }

        SpotifyItem? item;
        try
        {
            item = await SpotifyController.GetCurrentlyPlayingItemAsync(user);
        }
        catch (SpotifyException ex)
        {
            Response.Append($"{ChatMessage.Username}, {ex.Message}");
            return;
        }

        if (item is not SpotifyTrack track)
        {
            Response.Append($"{ChatMessage.Username}, {Texts.YouArentListeningToATrack}");
            return;
        }

        SpotifyUser[] targets = GetTargets();
        if (targets.Length == 0)
        {
            Response.Append($"{ChatMessage.Username}, {Texts.NoneOfTheGivenUsersAreRegistered}");
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
        return targets
            .Select(static t => StringHelpers.TrimAll(t))
            .Distinct()
            .Select(t => twitchBot.SpotifyUsers[t]).Where(static t => t is not null)
            .Take(5)
            .ToArray()!;
    }

    private void CreateMultipleTargetResponse(Dictionary<SpotifyUser, string?> success, SpotifyTrack track)
    {
        string[] successUsers = success.Where(static t => t.Value is null).Select(static t => t.Key.Username.Antiping()).ToArray();
        string[] failedUsers = success.Where(static t => t.Value is not null).Select(static t => t.Value!).ToArray();

        Span<char> joinBuffer = stackalloc char[512];
        int bufferLength;
        if (successUsers.Length != 0)
        {
            Response.Append($"{ChatMessage.Username}, {track} || {(track.IsLocal ? LocalFileDescription : track.Uri)}");

            bufferLength = StringHelpers.Join(", ", successUsers, joinBuffer);
            Response.Append($" has been added to the queue{(successUsers.Length > 1 ? "s of " : " of ")} {joinBuffer[..bufferLength]}");

            if (failedUsers.Length == 0)
            {
                return;
            }

            bufferLength = StringHelpers.Join(", ", failedUsers, joinBuffer);
            Response.Append(". ");
            Response.Append(joinBuffer[..bufferLength]);
        }
        else if (failedUsers.Length == 1)
        {
            Response.Append($"{ChatMessage.Username}, {failedUsers[0]}");
        }
        else
        {
            Response.Append($"{ChatMessage.Username}, {track} || {(track.IsLocal ? LocalFileDescription : track.Uri)}");
            Response.Append(" hasn't been added to any queue");
            if (failedUsers.Length == 0)
            {
                return;
            }

            bufferLength = StringHelpers.Join(", ", failedUsers, joinBuffer);
            Response.Append(". ");
            Response.Append(joinBuffer[..bufferLength]);
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
