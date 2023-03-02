using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HLE.Twitch;
using HLE.Twitch.Models;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Spotify;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Skip)]
public readonly ref struct SkipCommand
{
    public ChatMessage ChatMessage { get; }

    private readonly ref MessageBuilder _response;

    private readonly TwitchBot _twitchBot;
    [SuppressMessage("ReSharper", "NotAccessedField.Local")]
    [SuppressMessage("CodeQuality", "IDE0052:Remove unread private members")]
    private readonly ReadOnlySpan<char> _prefix;
    [SuppressMessage("ReSharper", "NotAccessedField.Local")]
    [SuppressMessage("CodeQuality", "IDE0052:Remove unread private members")]
    private readonly ReadOnlySpan<char> _alias;

    public SkipCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
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
        if (!ChatMessage.IsModerator && !messageExtension.IsBroadcaster)
        {
            _response.Append(ChatMessage.Username, ", ", Messages.YouArentAModeratorOrTheBroadcaster);
            return;
        }

        SpotifyUser? user = _twitchBot.SpotifyUsers[ChatMessage.Channel];
        if (user is null)
        {
            _response.Append(ChatMessage.Username, ", ", "you can't skip songs of ", ChatMessage.Channel.Antiping(), ", they have to register first");
            return;
        }

        try
        {
            SpotifyController.SkipAsync(user).Wait();
            _response.Append(ChatMessage.Username, ", ", "skipped to the next song in ", ChatMessage.Channel.Antiping(), "'s queue");
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

        List<SpotifyUser> usersToRemove = new();
        ListeningSession? listeningSession = SpotifyController.GetListeningSession(user);
        if (listeningSession is null)
        {
            return;
        }

        foreach (SpotifyUser listener in listeningSession.Listeners)
        {
            try
            {
                SpotifyController.ListenAlongWithAsync(listener, user).Wait();
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
}
