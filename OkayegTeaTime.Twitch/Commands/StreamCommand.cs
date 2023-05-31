using System;
using System.Collections.Frozen;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE.Numerics;
using HLE.Twitch.Models;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Api.Helix.Models;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Stream, typeof(StreamCommand))]
public readonly struct StreamCommand : IChatCommand<StreamCommand>
{
    public ResponseBuilder Response { get; }

    public ChatMessage ChatMessage { get; }

    private readonly TwitchBot _twitchBot;
    private readonly ReadOnlyMemory<char> _prefix;
    private readonly ReadOnlyMemory<char> _alias;

    private static readonly FrozenSet<long> _noViewerCountChannelIds = new long[]
    {
        149489313,
        35933008
    }.ToFrozenSet(true);

    public StreamCommand(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    {
        ChatMessage = chatMessage;
        Response = new(AppSettings.MaxMessageLength);
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public static void Create(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out StreamCommand command)
    {
        command = new(twitchBot, chatMessage, prefix, alias);
    }

    public async ValueTask Handle()
    {
        Regex channelPattern = _twitchBot.RegexCreator.Create(_alias.Span, _prefix.Span, @"\s\w+");
        Response.Append(ChatMessage.Username, ", ");

        using ChatMessageExtension messageExtension = new(ChatMessage);
        string channel = channelPattern.IsMatch(ChatMessage.Message) ? new(messageExtension.Split[1].Span) : ChatMessage.Username;

        Stream? stream = await _twitchBot.TwitchApi.GetStreamAsync(channel);
        if (stream is null)
        {
            Response.Append(Messages.ThisChannelIsCurrentlyNotStreaming);
            return;
        }

        Response.Append(stream.Username, " is currently streaming ", stream.GameName);
        if (ChatMessage.ChannelId != stream.UserId || !_noViewerCountChannelIds.Contains(stream.UserId))
        {
            Response.Append(" with ");
            Response.Advance(NumberHelper.InsertThousandSeparators(stream.ViewerCount, '.', Response.FreeBufferSpan));
            Response.Append(" viewer", stream.ViewerCount != 1 ? "s" : string.Empty);
        }

        TimeSpan streamTime = DateTime.UtcNow - stream.StartedAt;
        Response.Append(" for ", streamTime.ToString("g").Split('.')[0]);
    }

    public void Dispose()
    {
        Response.Dispose();
    }
}
