using System;
using System.Diagnostics.CodeAnalysis;
using HLE;
using HLE.Emojis;
using HLE.Twitch.Models;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Help)]
[SuppressMessage("ReSharper", "NotAccessedField.Local")]
[SuppressMessage("CodeQuality", "IDE0052:Remove unread private members")]
public readonly unsafe ref struct HelpCommand
{
    public ChatMessage ChatMessage { get; }

    public StringBuilder* Response { get; }

    private readonly TwitchBot _twitchBot;
    private readonly string? _prefix;
    private readonly string _alias;

    public HelpCommand(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
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
        ReadOnlySpan<char> username = messageExtension.Split.Length > 1 ? messageExtension.LowerSplit[1] : ChatMessage.Username;
        Response->Append(Emoji.PointRight, StringHelper.Whitespace, username, Messages.CommaSpace, "here you can find a list of commands and the repository: ", AppSettings.RepositoryUrl);
    }
}
