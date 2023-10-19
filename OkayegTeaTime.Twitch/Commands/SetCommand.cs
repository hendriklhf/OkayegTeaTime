using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE.Strings;
using HLE.Twitch.Models;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using Channel = OkayegTeaTime.Database.Models.Channel;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Set, typeof(SetCommand))]
public readonly partial struct SetCommand(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    : IChatCommand<SetCommand>
{
    public PooledStringBuilder Response { get; } = new(GlobalSettings.MaxMessageLength);

    public IChatMessage ChatMessage { get; } = chatMessage;

    private readonly TwitchBot _twitchBot = twitchBot;
    private readonly ReadOnlyMemory<char> _prefix = prefix;
    private readonly ReadOnlyMemory<char> _alias = alias;

    public static void Create(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out SetCommand command)
        => command = new(twitchBot, chatMessage, prefix, alias);

    public ValueTask HandleAsync()
    {
        ReadOnlySpan<char> alias = _alias.Span;
        ReadOnlySpan<char> prefix = _prefix.Span;
        Regex pattern = _twitchBot.MessageRegexCreator.Create(alias, prefix, @"\sprefix\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            SetPrefix();
            return ValueTask.CompletedTask;
        }

        pattern = _twitchBot.MessageRegexCreator.Create(alias, prefix, @"\semote\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            SetEmote();
            return ValueTask.CompletedTask;
        }

        pattern = _twitchBot.MessageRegexCreator.Create(alias, prefix, @"\s(sr|songrequests?)\s((1|true|enabled?)|(0|false|disabled?))");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            SetSongRequestState();
            return ValueTask.CompletedTask;
        }

        pattern = _twitchBot.MessageRegexCreator.Create(alias, prefix, @"\slocation\s((private)|(public))\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            SetLocation();
        }

        return ValueTask.CompletedTask;
    }

    private void SetPrefix()
    {
        using ChatMessageExtension messageExtension = new(ChatMessage);
        if (!ChatMessage.IsModerator && !messageExtension.IsBroadcaster)
        {
            Response.Append(ChatMessage.Username, ", ", Messages.YouArentAModeratorOrTheBroadcaster);
            return;
        }

        Channel? channel = _twitchBot.Channels[ChatMessage.ChannelId];
        if (channel is null)
        {
            Response.Append(ChatMessage.Username, ", ", Messages.AnErrorOccurredWhileTryingToSetThePrefix);
            return;
        }

        ReadOnlySpan<char> prefixSpan = messageExtension.LowerSplit[2].Span;
        if (prefixSpan.Length > GlobalSettings.MaxPrefixLength)
        {
            prefixSpan = prefixSpan[..GlobalSettings.MaxPrefixLength];
        }

        string prefix = new(prefixSpan);
        channel.Prefix = prefix;
        Response.Append(ChatMessage.Username, ", ", "prefix set to: ", prefix);
    }

    private void SetEmote()
    {
        using ChatMessageExtension messageExtension = new(ChatMessage);
        if (!ChatMessage.IsModerator && !messageExtension.IsBroadcaster)
        {
            Response.Append(ChatMessage.Username, Messages.YouArentAModeratorOrTheBroadcaster);
            return;
        }

        Channel? channel = _twitchBot.Channels[ChatMessage.ChannelId];
        if (channel is null)
        {
            Response.Append(ChatMessage.Username, ", ", Messages.AnErrorOccurredWhileTryingToSetTheEmote);
            return;
        }

        ReadOnlySpan<char> emoteSpan = messageExtension.Split[2].Span;
        if (emoteSpan.Length > GlobalSettings.MaxEmoteInFrontLength)
        {
            emoteSpan = emoteSpan[..GlobalSettings.MaxPrefixLength];
        }

        string emote = new(emoteSpan);
        channel.Emote = emote;
        Response.Append(ChatMessage.Username, ", ", "emote set to: ", emote);
    }

    private void SetSongRequestState()
    {
        Response.Append(ChatMessage.Username, ", ");
        using ChatMessageExtension messageExtension = new(ChatMessage);
        if (!ChatMessage.IsModerator && !messageExtension.IsBroadcaster)
        {
            Response.Append(Messages.YouHaveToBeAModOrTheBroadcasterToSetSongRequestSettings);
            return;
        }

        bool? state = null;
        if (GetEnabledPattern().IsMatch(messageExtension.Split[2].Span))
        {
            state = true;
        }
        else if (GetDisabledPattern().IsMatch(messageExtension.Split[2].Span))
        {
            state = false;
        }

        if (state is null)
        {
            Response.Append(Messages.TheStateCanOnlyBeSetToEnabledOrDisabled);
            return;
        }

        SpotifyUser? user = _twitchBot.SpotifyUsers[ChatMessage.Channel];
        if (user is null)
        {
            Response.Append("channel ", ChatMessage.Channel, " is not registered, they have to register first");
            return;
        }

        user.AreSongRequestsEnabled = state.Value;
        Response.Append("song requests ", state.Value ? "enabled" : "disabled", " ", "for channel ", ChatMessage.Channel);
    }

    [SkipLocalsInit]
    private void SetLocation()
    {
        using ChatMessageExtension messageExtension = new(ChatMessage);
        ReadOnlySpan<ReadOnlyMemory<char>> splits = messageExtension.Split.Splits;

        Span<char> cityBuffer = stackalloc char[512];
        int cityBufferLength = StringHelper.Join(splits[3..messageExtension.Split.Length], ' ', cityBuffer);
        string city = new(cityBuffer[..cityBufferLength]);

        bool isPrivate = messageExtension.LowerSplit[2].Span[1] == 'r';
        User? user = _twitchBot.Users[ChatMessage.UserId];
        if (user is null)
        {
            user = new(ChatMessage.UserId, ChatMessage.Username)
            {
                Location = city,
                IsPrivateLocation = isPrivate
            };
            _twitchBot.Users.Add(user);
        }
        else
        {
            user.Location = city;
            user.IsPrivateLocation = isPrivate;
        }

        Response.Append(ChatMessage.Username, ", your ", isPrivate ? "private" : "public", " location has been set");
    }

    [GeneratedRegex("(1|true|enabled?)", RegexOptions.Compiled | RegexOptions.IgnoreCase, 1000)]
    private static partial Regex GetEnabledPattern();

    [GeneratedRegex("(0|false|disabled?)", RegexOptions.Compiled | RegexOptions.IgnoreCase, 1000)]
    private static partial Regex GetDisabledPattern();

    public void Dispose() => Response.Dispose();

    public bool Equals(SetCommand other) =>
        _twitchBot.Equals(other._twitchBot) && _prefix.Equals(other._prefix) && _alias.Equals(other._alias) &&
        Response.Equals(other.Response) && ChatMessage.Equals(other.ChatMessage);

    public override bool Equals(object? obj) => obj is SetCommand other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(_twitchBot, _prefix, _alias, Response, ChatMessage);

    public static bool operator ==(SetCommand left, SetCommand right) => left.Equals(right);

    public static bool operator !=(SetCommand left, SetCommand right) => !left.Equals(right);
}
