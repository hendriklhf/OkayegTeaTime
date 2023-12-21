using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HLE.Strings;
using HLE.Twitch.Models;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Spotify;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Skip, typeof(SkipCommand))]
public readonly struct SkipCommand(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    : IChatCommand<SkipCommand>
{
    public PooledStringBuilder Response { get; } = new(GlobalSettings.MaxMessageLength);

    public IChatMessage ChatMessage { get; } = chatMessage;

    private readonly TwitchBot _twitchBot = twitchBot;
    private readonly ReadOnlyMemory<char> _prefix = prefix;
    private readonly ReadOnlyMemory<char> _alias = alias;

    public static void Create(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out SkipCommand command)
        => command = new(twitchBot, chatMessage, prefix, alias);

    public async ValueTask Handle()
    {
        if (GlobalSettings.Settings.Spotify is null)
        {
            Response.Append(ChatMessage.Username, ", ", Messages.TheCommandHasNotBeenConfiguredByTheBotOwner);
            return;
        }

        using ChatMessageExtension messageExtension = new(ChatMessage);
        if (!ChatMessage.IsModerator && !messageExtension.IsBroadcaster)
        {
            Response.Append(ChatMessage.Username, ", ", Messages.YouArentAModeratorOrTheBroadcaster);
            return;
        }

        SpotifyUser? user = _twitchBot.SpotifyUsers[ChatMessage.Channel];
        if (user is null)
        {
            Response.Append(ChatMessage.Username, ", ", "you can't skip songs of ", ChatMessage.Channel.Antiping(), ", they have to register first");
            return;
        }

        try
        {
            await SpotifyController.SkipAsync(user);
            Response.Append(ChatMessage.Username, ", ", "skipped to the next song in ", ChatMessage.Channel.Antiping(), "'s queue");
        }
        catch (SpotifyException ex)
        {
            Response.Append(ChatMessage.Username, ", ", ex.Message);
            return;
        }

        List<SpotifyUser> usersToRemove = [];
        ListeningSession? listeningSession = SpotifyController.GetListeningSession(user);
        if (listeningSession is null)
        {
            return;
        }

        foreach (SpotifyUser listener in listeningSession.Listeners)
        {
            try
            {
                await SpotifyController.ListenAlongWithAsync(listener, user);
            }
            catch (Exception)
            {
                usersToRemove.Add(listener);
            }
        }

        foreach (SpotifyUser u in usersToRemove)
        {
            listeningSession.Listeners.Remove(u);
        }
    }

    public void Dispose() => Response.Dispose();

    public bool Equals(SkipCommand other) =>
        _twitchBot.Equals(other._twitchBot) && _prefix.Equals(other._prefix) && _alias.Equals(other._alias) &&
        Response.Equals(other.Response) && ChatMessage.Equals(other.ChatMessage);

    public override bool Equals(object? obj) => obj is SkipCommand other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(_twitchBot, _prefix, _alias, Response, ChatMessage);

    public static bool operator ==(SkipCommand left, SkipCommand right) => left.Equals(right);

    public static bool operator !=(SkipCommand left, SkipCommand right) => !left.Equals(right);
}
