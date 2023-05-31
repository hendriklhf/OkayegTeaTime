using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using HLE.Emojis;
using HLE.Twitch.Models;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

#pragma warning disable IDE0052

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Help, typeof(HelpCommand))]
[SuppressMessage("ReSharper", "NotAccessedField.Local")]
[SuppressMessage("CodeQuality", "IDE0052:Remove unread private members")]
public readonly struct HelpCommand : IChatCommand<HelpCommand>
{
    public ResponseBuilder Response { get; }

    public ChatMessage ChatMessage { get; }

    private readonly TwitchBot _twitchBot;
    private readonly ReadOnlyMemory<char> _prefix;
    private readonly ReadOnlyMemory<char> _alias;

    public HelpCommand(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    {
        ChatMessage = chatMessage;
        Response = new(AppSettings.MaxMessageLength);
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public static void Create(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out HelpCommand command)
    {
        command = new(twitchBot, chatMessage, prefix, alias);
    }

    public ValueTask Handle()
    {
        using ChatMessageExtension messageExtension = new(ChatMessage);
        ReadOnlySpan<char> username = messageExtension.Split.Length > 1 ? messageExtension.LowerSplit[1].Span : ChatMessage.Username;
        Response.Append(Emoji.PointRight, " ", username, ", here you can find a list of commands and the repository: ", AppSettings.RepositoryUrl);
        return ValueTask.CompletedTask;
    }

    public void Dispose()
    {
        Response.Dispose();
    }
}
