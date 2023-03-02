using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using HLE.Twitch;
using HLE.Twitch.Models;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Spotify;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;
using StringHelper = HLE.StringHelper;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Spotify)]
[SuppressMessage("ReSharper", "NotAccessedField.Local")]
public readonly ref struct SpotifyCommand
{
    public ChatMessage ChatMessage { get; }

    private readonly TwitchBot _twitchBot;
    private readonly ref MessageBuilder _response;
    private readonly ReadOnlySpan<char> _prefix;
    private readonly ReadOnlySpan<char> _alias;

    public SpotifyCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        ChatMessage = chatMessage;
        _response = ref response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        using ChatMessageExtension messageExtension = new(ChatMessage);
        string username = new(messageExtension.LowerSplit.Length > 1 ? (messageExtension.LowerSplit[1].Equals("me", StringComparison.Ordinal) ? ChatMessage.Username : messageExtension.LowerSplit[1]) : ChatMessage.Channel);
        bool targetIsSender = username == ChatMessage.Username;
        SpotifyUser? user = _twitchBot.SpotifyUsers[username];
        if (user is null)
        {
            if (targetIsSender)
            {
                _response.Append(ChatMessage.Username, ", ", "can't get your currently playing song, you have to register first");
                return;
            }

            _response.Append(ChatMessage.Username, ", ", "can't get the currently playing song of ", username.Antiping(), ", they have to register first");
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

        _response.Append(ChatMessage.Username, ", ");
        switch (item)
        {
            case null:
            {
                _response.Append(targetIsSender ? "you aren't listening to anything" : $"{username.Antiping()} is not listening to anything");
                return;
            }
            case SpotifyTrack track:
            {
                string[] artists = track.Artists.Select(a => a.Name).ToArray();
                Span<char> joinBuffer = stackalloc char[250];
                int bufferLength = StringHelper.Join(artists, ", ", joinBuffer);
                _response.Append(track.Name, " by ", joinBuffer[..bufferLength], " || ", track.IsLocal ? "local file" : track.Uri);
                return;
            }
            case SpotifyEpisode episode:
            {
                _response.Append(episode.Name, " by ", episode.Show.Name, " || ", episode.IsLocal ? "local file" : episode.Uri);
                return;
            }
            default:
            {
                _response.Append(Messages.ListeningToAnUnknownSpotifyItemTypeMonkaS);
                return;
            }
        }
    }
}
