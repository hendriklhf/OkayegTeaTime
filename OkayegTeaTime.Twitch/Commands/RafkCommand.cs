using System;
using System.Diagnostics.CodeAnalysis;
using HLE.Strings;
using HLE.Twitch.Models;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Models.Json;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

#pragma warning disable IDE0052

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Rafk)]
[SuppressMessage("ReSharper", "NotAccessedField.Local")]
public readonly ref struct RafkCommand
{
    public ChatMessage ChatMessage { get; }

    private readonly ref PoolBufferStringBuilder _response;

    private readonly TwitchBot _twitchBot;
    private readonly ReadOnlySpan<char> _prefix;
    private readonly ReadOnlySpan<char> _alias;

    public RafkCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref PoolBufferStringBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        ChatMessage = chatMessage;
        _response = ref response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        User? user = _twitchBot.Users.Get(ChatMessage.UserId, ChatMessage.Username);
        if (user is null)
        {
            _response.Append(ChatMessage.Username, ", ", Messages.CantResumeYourAfkStatusBecauseYouNeverWentAfkBefore);
            return;
        }

        user.IsAfk = true;
        AfkCommand cmd = _twitchBot.CommandController[user.AfkType];
        _response.Append(new AfkMessage(user, cmd).Resuming);
    }
}
