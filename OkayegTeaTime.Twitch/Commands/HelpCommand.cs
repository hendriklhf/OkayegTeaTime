using System.Diagnostics.CodeAnalysis;
using HLE;
using HLE.Emojis;
using OkayegTeaTime.Files;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Help)]
[SuppressMessage("ReSharper", "NotAccessedField.Local")]
[SuppressMessage("CodeQuality", "IDE0052:Remove unread private members")]
public readonly unsafe ref struct HelpCommand
{
    public TwitchChatMessage ChatMessage { get; }

    public StringBuilder* Response { get; }

    private readonly TwitchBot _twitchBot;
    private readonly string? _prefix;
    private readonly string _alias;

    public HelpCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        ChatMessage = chatMessage;
        Response = response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        string username = ChatMessage.Split.Length > 1 ? ChatMessage.LowerSplit[1] : ChatMessage.Username;
        Response->Append(Emoji.PointRight, StringHelper.Whitespace, username, Messages.CommaSpace, "here you can find a list of commands and the repository: ", AppSettings.RepositoryUrl);
    }
}
