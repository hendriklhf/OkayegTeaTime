using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using HLE;
using HLE.Http;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Chatters)]
public readonly unsafe ref struct ChattersCommand
{
    public TwitchChatMessage ChatMessage { get; }

    public Response* Response { get; }

    [SuppressMessage("ReSharper", "NotAccessedField.Local")]
    [SuppressMessage("CodeQuality", "IDE0052:Remove unread private members")]
    private readonly TwitchBot _twitchBot;
    private readonly string? _prefix;
    private readonly string _alias;

    public ChattersCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, Response* response, string? prefix, string alias)
    {
        ChatMessage = chatMessage;
        Response = response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        Regex pattern = PatternCreator.Create(_alias, _prefix);
        if (pattern.IsMatch(ChatMessage.Message))
        {
            string channel = ChatMessage.LowerSplit.Length > 1 ? ChatMessage.LowerSplit[1] : ChatMessage.Channel;
            int chatterCount = GetChatterCount(channel);

            switch (chatterCount)
            {
                case > 1:
                    Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, "there are ", NumberHelper.InsertKDots(chatterCount), " chatters in the channel of ", channel.Antiping());
                    break;
                case > 0:
                    Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, "there is ", NumberHelper.InsertKDots(chatterCount), " chatter in the channel of ", channel.Antiping());
                    break;
                default:
                    Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, "there are no chatters in the channel of ", channel.Antiping());
                    break;
            }
        }
    }

    private static int GetChatterCount(string channel)
    {
        HttpGet request = new($"https://tmi.twitch.tv/group/user/{channel}/chatters");
        return !request.IsValidJsonData ? 0 : request.Data.GetProperty("chatter_count").GetInt32();
    }
}
