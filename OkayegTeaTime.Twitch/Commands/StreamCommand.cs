using System;
using System.Linq;
using System.Text.RegularExpressions;
using HLE.Numbers;
using HLE.Strings;
using HLE.Time;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;
using Stream = TwitchLib.Api.Helix.Models.Streams.GetStreams.Stream;

namespace OkayegTeaTime.Twitch.Commands;

public class StreamCommand : Command
{
    private readonly long[] _noViewerCount =
    {
        149489313
    };

    public StreamCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex channelPattern = PatternCreator.Create(Alias, Prefix, @"\s\w+");
        Response = $"{ChatMessage.Username}, ";

        Stream? stream = channelPattern.IsMatch(ChatMessage.Message) ? TwitchApi.GetStream(ChatMessage.Split[1]) : TwitchApi.GetStream(ChatMessage.ChannelId);
        if (stream is null)
        {
            Response += "this channel is currently not streaming";
            return;
        }

        Response += $"{stream.UserName} is currently streaming {stream.GameName} ";
        long userId = stream.UserId.ToLong();
        if (ChatMessage.ChannelId != userId || !_noViewerCount.Contains(userId))
        {
            Response += $"with {stream.ViewerCount} viewer{(stream.ViewerCount > 1 ? 's' : string.Empty)} ";
        }

        Response += "for ";
        TimeSpan streamSpan = stream.StartedAt.Subtract(DateTime.Now);
        long milliseconds = streamSpan.TotalMilliseconds.ToLong() + TimeHelper.Now();
        if (!DateTime.Now.IsDaylightSavingTime())
        {
            milliseconds += new Hour().Milliseconds;
        }

        string streamTime = TimeHelper.GetUnixDifference(milliseconds).ToString();
        Response += streamTime;
    }
}
