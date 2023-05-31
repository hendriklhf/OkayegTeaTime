using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using HLE.Twitch.Models;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Spotify;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;
using StringHelper = HLE.Strings.StringHelper;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Spotify, typeof(SpotifyCommand))]
[SuppressMessage("ReSharper", "NotAccessedField.Local")]
[SuppressMessage("CodeQuality", "IDE0052:Remove unread private members")]
public readonly struct SpotifyCommand : IChatCommand<SpotifyCommand>
{
    public ResponseBuilder Response { get; }

    public ChatMessage ChatMessage { get; }

    private readonly TwitchBot _twitchBot;
    private readonly ReadOnlyMemory<char> _prefix;
    private readonly ReadOnlyMemory<char> _alias;

    public SpotifyCommand(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    {
        ChatMessage = chatMessage;
        Response = new(AppSettings.MaxMessageLength);
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public static void Create(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out SpotifyCommand command)
    {
        command = new(twitchBot, chatMessage, prefix, alias);
    }

    public async ValueTask Handle()
    {
        using ChatMessageExtension messageExtension = new(ChatMessage);
        ReadOnlyMemory<char> firstParameter = messageExtension.LowerSplit[1];
        string username = new(messageExtension.LowerSplit.Length > 1 ? (firstParameter.Span is "me" ? ChatMessage.Username : firstParameter.Span) : ChatMessage.Channel);
        bool targetIsSender = username == ChatMessage.Username;
        SpotifyUser? user = _twitchBot.SpotifyUsers[username];
        if (user is null)
        {
            if (targetIsSender)
            {
                Response.Append(ChatMessage.Username, ", ", "can't get your currently playing song, you have to register first");
                return;
            }

            Response.Append(ChatMessage.Username, ", ", "can't get the currently playing song of ", username.Antiping(), ", they have to register first");
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

        Response.Append(ChatMessage.Username, ", ");
        switch (item)
        {
            case null:
            {
                Response.Append(targetIsSender ? "you aren't listening to anything" : $"{username.Antiping()} is not listening to anything");
                return;
            }
            case SpotifyTrack track:
            {
                Response.Append(track.Name, " by ");

                string[] artists = track.Artists.Select(a => a.Name).ToArray();
                int joinLength = StringHelper.Join(artists, ", ", Response.FreeBufferSpan);
                Response.Advance(joinLength);

                Response.Append(" || ", track.IsLocal ? "local file" : track.Uri);
                return;
            }
            case SpotifyEpisode episode:
            {
                Response.Append(episode.Name, " by ", episode.Show.Name, " || ", episode.IsLocal ? "local file" : episode.Uri);
                return;
            }
            default:
            {
                Response.Append(Messages.ListeningToAnUnknownSpotifyItemTypeMonkaS);
                return;
            }
        }
    }

    public void Dispose()
    {
        Response.Dispose();
    }
}
