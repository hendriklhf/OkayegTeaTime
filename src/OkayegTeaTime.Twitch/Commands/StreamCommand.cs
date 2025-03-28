﻿using System;
using System.Collections.Frozen;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE.Text;
using HLE.Twitch.Tmi.Models;
using OkayegTeaTime.Configuration;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Helix.Models;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand<StreamCommand>(CommandType.Stream)]
public readonly struct StreamCommand(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    : IChatCommand<StreamCommand>
{
    public PooledStringBuilder Response { get; } = new(GlobalSettings.MaxMessageLength);

    public ChatMessage ChatMessage { get; } = chatMessage;

    private readonly TwitchBot _twitchBot = twitchBot;
    private readonly ReadOnlyMemory<char> _prefix = prefix;
    private readonly ReadOnlyMemory<char> _alias = alias;

    private static readonly FrozenSet<long> s_noViewerCountChannelIds = new long[]
    {
        149_489_313,
        35_933_008
    }.ToFrozenSet();

    public static void Create(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out StreamCommand command)
        => command = new(twitchBot, chatMessage, prefix, alias);

    public async ValueTask HandleAsync()
    {
        Regex channelPattern = _twitchBot.MessageRegexCreator.Create(_alias.Span, _prefix.Span, @"\s\w+");
        Response.Append($"{ChatMessage.Username}, ");

        using ChatMessageExtension messageExtension = new(ChatMessage);
        string channel = channelPattern.IsMatch(ChatMessage.Message.AsSpan()) ? new(messageExtension.Split[1].Span) : ChatMessage.Username.ToString();

        Stream? stream = await _twitchBot.TwitchApi.GetStreamAsync(channel);
        if (stream is null)
        {
            Response.Append(Texts.ThisChannelIsCurrentlyNotStreaming);
            return;
        }

        Response.Append($"{stream.Username} is currently streaming \"{stream.GameName}\"");
        if (ChatMessage.ChannelId != stream.UserId || !s_noViewerCountChannelIds.Contains(stream.UserId))
        {
            Response.Append(" for ");
            Response.Append(stream.ViewerCount, "N0");
            Response.Append(" viewer");
            if (stream.ViewerCount != 1)
            {
                Response.Append('s');
            }
        }

#pragma warning disable S6354
        TimeSpan streamTime = DateTime.UtcNow - stream.StartedAt;
#pragma warning restore S6354

        Response.Append(" since ");
        TimeSpanFormatter.Format(streamTime, Response);
    }

    public void Dispose() => Response.Dispose();

    public bool Equals(StreamCommand other) =>
        _twitchBot.Equals(other._twitchBot) && _prefix.Equals(other._prefix) && _alias.Equals(other._alias) &&
        Response.Equals(other.Response) && ChatMessage.Equals(other.ChatMessage);

    public override bool Equals(object? obj) => obj is StreamCommand other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(_twitchBot, _prefix, _alias, Response, ChatMessage);

    public static bool operator ==(StreamCommand left, StreamCommand right) => left.Equals(right);

    public static bool operator !=(StreamCommand left, StreamCommand right) => !left.Equals(right);
}
