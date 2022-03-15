using System.Text.RegularExpressions;
using HLE.Numbers;
using HLE.Time;
using OkayegTeaTime.Twitch.Api;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;
using TwitchLib.Api.Helix.Models.Streams.GetStreams;

namespace OkayegTeaTime.Twitch.Commands.CommandClasses;

public class StreamCommand : Command
{
    public StreamCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex channelPattern = PatternCreator.Create(Alias, Prefix, @"\s\w+");
        Stream? stream;
        Response = $"{ChatMessage.Username}, ";

        if (channelPattern.IsMatch(ChatMessage.Message))
        {
            stream = TwitchApi.GetStream(ChatMessage.Split[1]);
        }
        else
        {
            stream = TwitchApi.GetStream(ChatMessage.ChannelId);
        }

        if (stream is null)
        {
            Response += "this channel is currently not streaming";
            return;
        }
        Response += $"{stream.UserName} is currently streaming {stream.GameName} with {stream.ViewerCount} viewer{(stream.ViewerCount > 1 ? 's' : string.Empty)} for ";
        TimeSpan streamSpan = stream.StartedAt.Subtract(DateTime.Now);
        long milliseconds = streamSpan.TotalMilliseconds.ToLong() + TimeHelper.Now();
        if (!DateTime.Now.IsDaylightSavingTime())
        {
            milliseconds += new Hour().Milliseconds;
        }
        string streamTime = TimeHelper.GetUnixDifference(milliseconds).ToString();
        Response += streamTime;
        return;
    }
}
