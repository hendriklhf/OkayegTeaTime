using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE.Memory;
using HLE.Strings;
using HLE.Twitch.Models;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using Channel = OkayegTeaTime.Database.Models.Channel;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Set, typeof(SetCommand))]
public readonly struct SetCommand : IChatCommand<SetCommand>
{
    public ResponseBuilder Response { get; }

    public ChatMessage ChatMessage { get; }

    private readonly TwitchBot _twitchBot;
    private readonly ReadOnlyMemory<char> _prefix;
    private readonly ReadOnlyMemory<char> _alias;

    private static readonly Regex _enabledPattern = new(@"(1|true|enabled?)", RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1));
    private static readonly Regex _disabledPattern = new(@"(0|false|disabled?)", RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1));

    public SetCommand(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    {
        ChatMessage = chatMessage;
        Response = new(AppSettings.MaxMessageLength);
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public static void Create(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out SetCommand command)
    {
        command = new(twitchBot, chatMessage, prefix, alias);
    }

    public ValueTask Handle()
    {
        ReadOnlySpan<char> alias = _alias.Span;
        ReadOnlySpan<char> prefix = _prefix.Span;
        Regex pattern = _twitchBot.RegexCreator.Create(alias, prefix, @"\sprefix\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            SetPrefix();
            return ValueTask.CompletedTask;
        }

        pattern = _twitchBot.RegexCreator.Create(alias, prefix, @"\semote\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            SetEmote();
            return ValueTask.CompletedTask;
        }

        pattern = _twitchBot.RegexCreator.Create(alias, prefix, @"\s(sr|songrequests?)\s((1|true|enabled?)|(0|false|disabled?))");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            SetSongRequestState();
            return ValueTask.CompletedTask;
        }

        pattern = _twitchBot.RegexCreator.Create(alias, prefix, @"\slocation\s((private)|(public))\s\S+");
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
        if (prefixSpan.Length > AppSettings.MaxPrefixLength)
        {
            prefixSpan = prefixSpan[..AppSettings.MaxPrefixLength];
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
        if (emoteSpan.Length > AppSettings.MaxEmoteInFrontLength)
        {
            emoteSpan = emoteSpan[..AppSettings.MaxPrefixLength];
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
        if (_enabledPattern.IsMatch(messageExtension.Split[2].Span))
        {
            state = true;
        }
        else if (_disabledPattern.IsMatch(messageExtension.Split[2].Span))
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

    private void SetLocation()
    {
        using ChatMessageExtension messageExtension = new(ChatMessage);
        using RentedArray<ReadOnlyMemory<char>> splits = messageExtension.Split.GetSplits();

        Span<char> cityBuffer = stackalloc char[500];
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

        Response.Append(ChatMessage.Username, ", ", "your ", isPrivate ? "private" : "public", " location has been set");
    }

    public void Dispose()
    {
        Response.Dispose();
    }
}
