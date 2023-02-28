using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HLE;
using HLE.Twitch.Models;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Spotify;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Skip)]
public readonly unsafe ref struct SkipCommand
{
    public ChatMessage ChatMessage { get; }

    public StringBuilder* Response { get; }

    private readonly TwitchBot _twitchBot;
    [SuppressMessage("ReSharper", "NotAccessedField.Local")]
    [SuppressMessage("CodeQuality", "IDE0052:Remove unread private members")]
    private readonly string? _prefix;
    [SuppressMessage("ReSharper", "NotAccessedField.Local")]
    [SuppressMessage("CodeQuality", "IDE0052:Remove unread private members")]
    private readonly string _alias;

    public SkipCommand(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        ChatMessage = chatMessage;
        Response = response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        using ChatMessageExtension messageExtension = new(ChatMessage);
        if (!ChatMessage.IsModerator && !messageExtension.IsBroadcaster)
        {
            Response->Append(ChatMessage.Username, Messages.CommaSpace, Messages.YouArentAModeratorOrTheBroadcaster);
            return;
        }

        SpotifyUser? user = _twitchBot.SpotifyUsers[ChatMessage.Channel];
        if (user is null)
        {
            Response->Append(ChatMessage.Username, Messages.CommaSpace, "you can't skip songs of ", ChatMessage.Channel.Antiping(), ", they have to register first");
            return;
        }

        try
        {
            SpotifyController.SkipAsync(user).Wait();
            Response->Append(ChatMessage.Username, Messages.CommaSpace, "skipped to the next song in ", ChatMessage.Channel.Antiping(), "'s queue");
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
