using System.Text.RegularExpressions;
using HLE;
using HLE.Emojis;
using HLE.Twitch.Models;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Fuck)]
public readonly unsafe ref struct FuckCommand
{
    public ChatMessage ChatMessage { get; }

    public StringBuilder* Response { get; }

    private readonly TwitchBot _twitchBot;
    private readonly string? _prefix;
    private readonly string _alias;

    public FuckCommand(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        ChatMessage = chatMessage;
        Response = response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        Regex pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\s\w+(\s\S+)?");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            using ChatMessageExtension messageExtension = new(ChatMessage);
            Response->Append(Emoji.PointRight, StringHelper.Whitespace, Emoji.OkHand, ChatMessage.Username, " fucked ", messageExtension.Split[1]);
            if (messageExtension.Split.Length > 2)
            {
                Response->Append(StringHelper.Whitespace, messageExtension.Split[2]);
            }
        }
    }
}
