using System;
using System.Text.Json;
using System.Text.RegularExpressions;
using HLE;
using HLE.Twitch;
using HLE.Twitch.Models;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Chatters)]
public readonly ref struct ChattersCommand
{
    public ChatMessage ChatMessage { get; }

    private readonly TwitchBot _twitchBot;
    private readonly ref MessageBuilder _response;
    private readonly ReadOnlySpan<char> _prefix;
    private readonly ReadOnlySpan<char> _alias;

    public ChattersCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        ChatMessage = chatMessage;
        _response = ref response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        Regex pattern = _twitchBot.RegexCreator.Create(_alias, _prefix);
        if (pattern.IsMatch(ChatMessage.Message))
        {
            using ChatMessageExtension messageExtension = new(ChatMessage);
            string channel = messageExtension.LowerSplit.Length > 1 ? new(messageExtension.LowerSplit[1]) : ChatMessage.Channel;
            int chatterCount = GetChatterCount(channel);

            _response.Append(ChatMessage.Username, ", ");
            switch (chatterCount)
            {
                case > 1:
                    _response.Append("there are ", NumberHelper.InsertKDots(chatterCount), " chatters in the channel of ", channel.Antiping());
                    break;
                case 1:
                    _response.Append("there is ", NumberHelper.InsertKDots(chatterCount), " chatter in the channel of ", channel.Antiping());
                    break;
                case 0:
                    _response.Append("there are no chatters in the channel of ", channel.Antiping());
                    break;
                default:
                    _response.Append(Messages.ApiError);
                    break;
            }
        }
    }

    private static int GetChatterCount(string channel)
    {
        HttpGet request = new($"https://tmi.twitch.tv/group/user/{channel}/chatters");
        if (request.Result is null)
        {
            return -1;
        }

        JsonElement json = JsonSerializer.Deserialize<JsonElement>(request.Result);
        return json.GetProperty("chatter_count").GetInt32();
    }
}
