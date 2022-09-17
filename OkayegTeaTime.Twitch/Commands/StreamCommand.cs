using System;
using System.Linq;
using System.Text.RegularExpressions;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;
using Stream = TwitchLib.Api.Helix.Models.Streams.GetStreams.Stream;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Stream)]
public sealed class StreamCommand : Command
{
    private readonly long[] _noViewerCount =
    {
        149489313
    };

    public StreamCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias) : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex channelPattern = PatternCreator.Create(_alias, _prefix, @"\s\w+");
        Response = $"{ChatMessage.Username}, ";

        Stream? stream = channelPattern.IsMatch(ChatMessage.Message) ? _twitchBot.TwitchApi.GetStream(ChatMessage.Split[1]) : _twitchBot.TwitchApi.GetStream(ChatMessage.ChannelId);
        if (stream is null)
        {
            Response += "this channel is currently not streaming";
            return;
        }

        Response += $"{stream.UserName} is currently streaming {stream.GameName} ";
        long userId = long.Parse(stream.UserId);
        if (ChatMessage.ChannelId != userId || !_noViewerCount.Contains(userId))
        {
            Response += $"with {stream.ViewerCount} viewer{(stream.ViewerCount > 1 ? 's' : string.Empty)} ";
        }

        TimeSpan streamTime = DateTime.UtcNow - stream.StartedAt;
        Response += $"for {streamTime.ToString("g").Split('.')[0]}";
    }
}
