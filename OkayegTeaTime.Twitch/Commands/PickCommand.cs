using System;
using System.Text.RegularExpressions;
using HLE.Twitch;
using HLE.Twitch.Models;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using Random = HLE.Random;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Pick)]
public readonly ref struct PickCommand
{
    public ChatMessage ChatMessage { get; }

    private readonly ref MessageBuilder _response;

    private readonly TwitchBot _twitchBot;
    private readonly string? _prefix;
    private readonly string _alias;

    public PickCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, string? prefix, string alias)
    {
        ChatMessage = chatMessage;
        _response = ref response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        Regex pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\s\S+");
        if (!pattern.IsMatch(ChatMessage.Message))
        {
            _response.Append(ChatMessage.Username, ", ", Messages.NoItemsProvided);
            return;
        }

        using ChatMessageExtension messageExtension = new(ChatMessage);
        int randomIndex = Random.Int(1, messageExtension.Split.Length - 1);
        ReadOnlySpan<char> randomPick = messageExtension.Split[randomIndex];
        _response.Append(ChatMessage.Username, ", ", randomPick);
    }
}
