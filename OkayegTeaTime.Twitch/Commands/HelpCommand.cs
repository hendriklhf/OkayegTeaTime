using System;
using System.Diagnostics.CodeAnalysis;
using HLE.Emojis;
using HLE.Twitch;
using HLE.Twitch.Models;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Help)]
[SuppressMessage("ReSharper", "NotAccessedField.Local")]
[SuppressMessage("CodeQuality", "IDE0052:Remove unread private members")]
public readonly ref struct HelpCommand
{
    public ChatMessage ChatMessage { get; }

    private readonly TwitchBot _twitchBot;
    private readonly ref MessageBuilder _response;
    private readonly ReadOnlySpan<char> _prefix;
    private readonly ReadOnlySpan<char> _alias;

    public HelpCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
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
        ReadOnlySpan<char> username = messageExtension.Split.Length > 1 ? messageExtension.LowerSplit[1] : ChatMessage.Username;
        _response.Append(Emoji.PointRight, " ", username, ", ", "here you can find a list of commands and the repository: ", AppSettings.RepositoryUrl);
    }
}
