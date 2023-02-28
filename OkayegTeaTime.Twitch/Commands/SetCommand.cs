using System;
using System.Text.RegularExpressions;
using HLE;
using HLE.Memory;
using HLE.Twitch.Models;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using Channel = OkayegTeaTime.Database.Models.Channel;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Set)]
public readonly unsafe ref struct SetCommand
{
    public ChatMessage ChatMessage { get; }

    public StringBuilder* Response { get; }

    private readonly TwitchBot _twitchBot;
    private readonly string? _prefix;
    private readonly string _alias;

    private static readonly Regex _enabledPattern = new(@"(1|true|enabled?)", RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1));
    private static readonly Regex _disabledPattern = new(@"(0|false|disabled?)", RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1));

    public SetCommand(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        ChatMessage = chatMessage;
        Response = response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        Regex pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\sprefix\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            using ChatMessageExtension messageExtension = new(ChatMessage);
            if (!ChatMessage.IsModerator && !messageExtension.IsBroadcaster)
            {
                Response->Append(ChatMessage.Username, Messages.CommaSpace, Messages.YouArentAModeratorOrTheBroadcaster);
                return;
            }

            Channel? channel = _twitchBot.Channels[ChatMessage.ChannelId];
            if (channel is null)
            {
                Response->Append(ChatMessage.Username, Messages.CommaSpace, Messages.AnErrorOccurredWhileTryingToSetThePrefix);
                return;
            }

            ReadOnlySpan<char> prefixSpan = messageExtension.LowerSplit[2];
            if (prefixSpan.Length > AppSettings.MaxPrefixLength)
            {
                prefixSpan = prefixSpan[..AppSettings.MaxPrefixLength];
            }

            string prefix = new(prefixSpan);
            channel.Prefix = prefix;
            Response->Append(ChatMessage.Username, Messages.CommaSpace, "prefix set to: ", prefix);
            return;
        }

        pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\semote\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            using ChatMessageExtension messageExtension = new(ChatMessage);
            if (!ChatMessage.IsModerator && !messageExtension.IsBroadcaster)
            {
                Response->Append(ChatMessage.Username, Messages.YouArentAModeratorOrTheBroadcaster);
                return;
            }

            Channel? channel = _twitchBot.Channels[ChatMessage.ChannelId];
            if (channel is null)
            {
                Response->Append(ChatMessage.Username, Messages.CommaSpace, Messages.AnErrorOccurredWhileTryingToSetTheEmote);
                return;
            }

            ReadOnlySpan<char> emoteSpan = messageExtension.Split[2];
            if (emoteSpan.Length > AppSettings.MaxEmoteInFrontLength)
            {
                emoteSpan = emoteSpan[..AppSettings.MaxPrefixLength];
            }

            string emote = new(emoteSpan);
            channel.Emote = emote;
            Response->Append(ChatMessage.Username, Messages.CommaSpace, "emote set to: ", emote);
            return;
        }

        pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\s(sr|songrequests?)\s((1|true|enabled?)|(0|false|disabled?))");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Response->Append(ChatMessage.Username, Messages.CommaSpace);
            using ChatMessageExtension messageExtension = new(ChatMessage);
            if (!ChatMessage.IsModerator && !messageExtension.IsBroadcaster)
            {
                Response->Append(Messages.YouHaveToBeAModOrTheBroadcasterToSetSongRequestSettings);
                return;
            }

            bool? state = null;
            if (_enabledPattern.IsMatch(messageExtension.Split[2]))
            {
                state = true;
            }
            else if (_disabledPattern.IsMatch(messageExtension.Split[2]))
            {
                state = false;
            }

            if (state is null)
            {
                Response->Append(Messages.TheStateCanOnlyBeSetToEnabledOrDisabled);
                return;
            }

            SpotifyUser? user = _twitchBot.SpotifyUsers[ChatMessage.Channel];
            if (user is null)
            {
                Response->Append("channel ", ChatMessage.Channel, " is not registered, they have to register first");
                return;
            }

            user.AreSongRequestsEnabled = state.Value;
            Response->Append("song requests ", state.Value ? "enabled" : "disabled", StringHelper.Whitespace, "for channel ", ChatMessage.Channel);
            return;
        }

        pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\slocation\s((private)|(public))\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            using ChatMessageExtension messageExtension = new(ChatMessage);
            using RentedArray<ReadOnlyMemory<char>> splits = messageExtension.Split.GetSplits();

            Span<char> cityBuffer = stackalloc char[500];
            int cityBufferLength = StringHelper.Join(splits, ' ', cityBuffer);
            string city = new(cityBuffer[..cityBufferLength]);

            bool isPrivate = messageExtension.LowerSplit[2][1] == 'r';
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

            Response->Append(ChatMessage.Username, Messages.CommaSpace, "your ", isPrivate ? "private" : "public", " location has been set");
        }
    }
}
