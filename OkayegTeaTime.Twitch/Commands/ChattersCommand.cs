using System;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE.Numerics;
using HLE.Twitch.Models;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

public readonly struct ChattersCommand : IChatCommand<ChattersCommand>
{
    public ResponseBuilder Response { get; }

    public ChatMessage ChatMessage { get; }

    private readonly TwitchBot _twitchBot;
    private readonly ReadOnlyMemory<char> _prefix;
    private readonly ReadOnlyMemory<char> _alias;

    public ChattersCommand(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    {
        ChatMessage = chatMessage;
        Response = new(AppSettings.MaxMessageLength);
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public static void Create(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out ChattersCommand command)
    {
        command = new(twitchBot, chatMessage, prefix, alias);
    }

    public async ValueTask Handle()
    {
        Regex pattern = _twitchBot.RegexCreator.Create(_alias.Span, _prefix.Span);
        if (pattern.IsMatch(ChatMessage.Message))
        {
            using ChatMessageExtension messageExtension = new(ChatMessage);
            string channel = messageExtension.LowerSplit.Length > 1 ? new(messageExtension.LowerSplit[1].Span) : ChatMessage.Channel;
            int chatterCount = await GetChatterCount(channel);

            Response.Append(ChatMessage.Username, ", ");
            switch (chatterCount)
            {
                case > 1:
                    Response.Append("there are ");
                    Response.Advance(NumberHelper.InsertThousandSeparators(chatterCount, '.', Response.FreeBufferSpan));
                    Response.Append(" chatters in the channel of ", channel.Antiping());
                    break;
                case 1:
                    Response.Append("there is ");
                    Response.Append(chatterCount);
                    Response.Append(" chatter in the channel of ", channel.Antiping());
                    break;
                case 0:
                    Response.Append("there are no chatters in the channel of ", channel.Antiping());
                    break;
                default:
                    Response.Append(Messages.ApiError);
                    break;
            }
        }
    }

    public void Dispose()
    {
        Response.Dispose();
    }

    private static async ValueTask<int> GetChatterCount(string channel)
    {
        HttpGet request = await HttpGet.GetStringAsync($"https://tmi.twitch.tv/group/user/{channel}/chatters");
        if (request.Result is null)
        {
            return -1;
        }

        JsonElement json = JsonSerializer.Deserialize<JsonElement>(request.Result);
        return json.GetProperty("chatter_count").GetInt32();
    }
}
