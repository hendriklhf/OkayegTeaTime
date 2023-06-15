using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using HLE.Twitch.Models;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

#pragma warning disable IDE0052

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Rafk, typeof(RafkCommand))]
[SuppressMessage("ReSharper", "NotAccessedField.Local")]
public readonly struct RafkCommand : IChatCommand<RafkCommand>
{
    public ResponseBuilder Response { get; }

    public ChatMessage ChatMessage { get; }

    private readonly TwitchBot _twitchBot;
    private readonly ReadOnlyMemory<char> _prefix;
    private readonly ReadOnlyMemory<char> _alias;

    public RafkCommand(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    {
        ChatMessage = chatMessage;
        Response = new(AppSettings.MaxMessageLength);
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public static void Create(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out RafkCommand command)
    {
        command = new(twitchBot, chatMessage, prefix, alias);
    }

    public ValueTask Handle()
    {
        User? user = _twitchBot.Users.Get(ChatMessage.UserId, ChatMessage.Username);
        if (user is null)
        {
            Response.Append(ChatMessage.Username, ", ", Messages.CantResumeYourAfkStatusBecauseYouNeverWentAfkBefore);
            return ValueTask.CompletedTask;
        }

        user.IsAfk = true;

        int afkMessageLength = _twitchBot.AfkMessageBuilder.BuildResumingMessage(ChatMessage.Username, user.AfkType, Response.FreeBufferSpan);
        Response.Advance(afkMessageLength);
        return ValueTask.CompletedTask;
    }

    public void Dispose()
    {
        Response.Dispose();
    }
}
