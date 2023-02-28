using System;
using System.Collections.Frozen;
using System.Text.RegularExpressions;
using HLE;
using HLE.Twitch.Models;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using Stream = TwitchLib.Api.Helix.Models.Streams.GetStreams.Stream;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Stream)]
public readonly unsafe ref struct StreamCommand
{
    public ChatMessage ChatMessage { get; }

    public StringBuilder* Response { get; }

    private readonly TwitchBot _twitchBot;
    private readonly string? _prefix;
    private readonly string _alias;

    private static readonly FrozenSet<long> _noViewerCountChannelIds = new long[]
    {
        149489313,
        35933008
    }.ToFrozenSet();

    public StreamCommand(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        ChatMessage = chatMessage;
        Response = response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        Regex channelPattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\s\w+");
        Response->Append(ChatMessage.Username, Messages.CommaSpace);

        using ChatMessageExtension messageExtension = new(ChatMessage);
        Stream? stream = channelPattern.IsMatch(ChatMessage.Message) ? _twitchBot.TwitchApi.GetStream(new string(messageExtension.Split[1])) : _twitchBot.TwitchApi.GetStream(ChatMessage.ChannelId);
        if (stream is null)
        {
            Response->Append(Messages.ThisChannelIsCurrentlyNotStreaming);
            return;
        }

        Response->Append(stream.UserName, " is currently streaming ", stream.GameName);
        long streamUserId = long.Parse(stream.UserId);
        if (ChatMessage.ChannelId != streamUserId || !_noViewerCountChannelIds.Contains(streamUserId))
        {
            Response->Append(" with ", NumberHelper.InsertKDots(stream.ViewerCount), " viewer", stream.ViewerCount != 1 ? "s" : string.Empty);
        }

        TimeSpan streamTime = DateTime.UtcNow - stream.StartedAt;
        Response->Append(" for ", streamTime.ToString("g").Split('.')[0]);
    }
}
