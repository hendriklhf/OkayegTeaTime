using System;
using System.Linq;
using System.Text.RegularExpressions;
using HLE;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;
using Stream = TwitchLib.Api.Helix.Models.Streams.GetStreams.Stream;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Stream)]
public readonly unsafe ref struct StreamCommand
{
    public TwitchChatMessage ChatMessage { get; }

    public Response* Response { get; }

    private readonly TwitchBot _twitchBot;
    private readonly string? _prefix;
    private readonly string _alias;

    private readonly long[] _noViewerCount =
    {
        149489313
    };

    public StreamCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, Response* response, string? prefix, string alias)
    {
        ChatMessage = chatMessage;
        Response = response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        Regex channelPattern = PatternCreator.Create(_alias, _prefix, @"\s\w+");
        Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace);

        Stream? stream = channelPattern.IsMatch(ChatMessage.Message) ? _twitchBot.TwitchApi.GetStream(ChatMessage.Split[1]) : _twitchBot.TwitchApi.GetStream(ChatMessage.ChannelId);
        if (stream is null)
        {
            Response->Append(PredefinedMessages.ThisChannelIsCurrentlyNotStreaming);
            return;
        }

        Response->Append(stream.UserName, " is currently streaming ", stream.GameName);
        long userId = long.Parse(stream.UserId);
        if (ChatMessage.ChannelId != userId || !_noViewerCount.Contains(userId))
        {
            Response->Append(" with ", NumberHelper.InsertKDots(stream.ViewerCount), " viewer", stream.ViewerCount != 1 ? "s" : string.Empty);
        }

        TimeSpan streamTime = DateTime.UtcNow - stream.StartedAt;
        Response->Append(" for ", streamTime.ToString("g").Split('.')[0]);
    }
}
