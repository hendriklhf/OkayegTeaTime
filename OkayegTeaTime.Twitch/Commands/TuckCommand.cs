using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using HLE;
using HLE.Emojis;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;
using StringHelper = HLE.StringHelper;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Tuck)]
public readonly unsafe ref struct TuckCommand
{
    public TwitchChatMessage ChatMessage { get; }

    public StringBuilder* Response { get; }

    [SuppressMessage("ReSharper", "NotAccessedField.Local")]
    [SuppressMessage("CodeQuality", "IDE0052:Remove unread private members")]
    private readonly TwitchBot _twitchBot;
    private readonly string? _prefix;
    private readonly string _alias;

    public TuckCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        ChatMessage = chatMessage;
        Response = response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        Regex pattern = PatternCreator.Create(_alias, _prefix, @"\s\w+(\s\S+)?");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            string target = ChatMessage.LowerSplit[1];
            Response->Append(Emoji.PointRight, StringHelper.Whitespace, Emoji.Bed, StringHelper.Whitespace, ChatMessage.Username);
            Response->Append(" tucked ", target, " to bed");
            string emote = ChatMessage.LowerSplit.Length > 2 ? ChatMessage.Split[2] : string.Empty;
            if (!string.IsNullOrWhiteSpace(emote))
            {
                Response->Append(StringHelper.Whitespace, emote);
            }
        }
    }
}
