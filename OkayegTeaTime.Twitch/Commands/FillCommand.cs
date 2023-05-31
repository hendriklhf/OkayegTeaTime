using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE.Twitch.Models;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Fill, typeof(FillCommand))]
public readonly struct FillCommand : IChatCommand<FillCommand>
{
    public ResponseBuilder Response { get; }

    public ChatMessage ChatMessage { get; }

    private readonly TwitchBot _twitchBot;
    private readonly ReadOnlyMemory<char> _prefix;
    private readonly ReadOnlyMemory<char> _alias;

    public FillCommand(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    {
        ChatMessage = chatMessage;
        Response = new(AppSettings.MaxMessageLength);
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public static void Create(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out FillCommand command)
    {
        command = new(twitchBot, chatMessage, prefix, alias);
    }

    public ValueTask Handle()
    {
        Regex pattern = _twitchBot.RegexCreator.Create(_alias.Span, _prefix.Span, @"\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            using ChatMessageExtension messageExtension = new(ChatMessage);
            string emote = _twitchBot.Channels[ChatMessage.ChannelId]?.Emote ?? AppSettings.DefaultEmote;
            int maxLength = AppSettings.MaxMessageLength - (emote.Length + 1);
            ReadOnlyMemory<char> nextMessagePart = messageExtension.Split[Random.Shared.Next(1, messageExtension.Split.Length)];
            for (int currentMessageLength = 0; currentMessageLength + nextMessagePart.Length + 1 < maxLength; currentMessageLength += nextMessagePart.Length + 1)
            {
                Response.Append(nextMessagePart.Span, " ");
                nextMessagePart = messageExtension.Split[Random.Shared.Next(1, messageExtension.Split.Length)];
            }
        }

        return ValueTask.CompletedTask;
    }

    public void Dispose()
    {
        Response.Dispose();
    }
}
