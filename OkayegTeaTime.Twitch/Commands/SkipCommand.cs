using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using HLE.Twitch.Models;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Spotify;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Skip, typeof(SkipCommand))]
[SuppressMessage("CodeQuality", "IDE0052:Remove unread private members")]
[SuppressMessage("ReSharper", "NotAccessedField.Local")]
public readonly struct SkipCommand : IChatCommand<SkipCommand>
{
    public ResponseBuilder Response { get; }

    public ChatMessage ChatMessage { get; }

    private readonly TwitchBot _twitchBot;
    private readonly ReadOnlyMemory<char> _prefix;
    private readonly ReadOnlyMemory<char> _alias;

    public SkipCommand(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    {
        ChatMessage = chatMessage;
        Response = new(AppSettings.MaxMessageLength);
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public static void Create(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out SkipCommand command)
    {
        command = new(twitchBot, chatMessage, prefix, alias);
    }

    public async ValueTask Handle()
    {
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

    public void Dispose()
    {
        Response.Dispose();
    }
}
