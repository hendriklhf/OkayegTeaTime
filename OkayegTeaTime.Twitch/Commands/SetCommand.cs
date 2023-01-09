using System;
using System.Text.RegularExpressions;
using HLE;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Files;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Set)]
public readonly unsafe ref struct SetCommand
{
    public TwitchChatMessage ChatMessage { get; }

    public StringBuilder* Response { get; }

    private readonly TwitchBot _twitchBot;
    private readonly string? _prefix;
    private readonly string _alias;

    private static readonly Regex _enabledPattern = new(@"(1|true|enabled?)", RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1));
    private static readonly Regex _disabledPattern = new(@"(0|false|disabled?)", RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1));

    public SetCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        ChatMessage = chatMessage;
        Response = response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        Regex pattern = PatternCreator.Create(_alias, _prefix, @"\sprefix\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            if (ChatMessage is { IsModerator: false, IsBroadcaster: false })
            {
                Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, PredefinedMessages.YouArentAModOrTheBroadcaster);
                return;
            }

            Channel? channel = _twitchBot.Channels[ChatMessage.ChannelId];
            if (channel is null)
            {
                Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, PredefinedMessages.AnErrorOccurredWhileTryingToSetThePrefix);
                return;
            }

            string prefix = ChatMessage.LowerSplit[2][..(ChatMessage.LowerSplit[2].Length > AppSettings.MaxPrefixLength ? AppSettings.MaxPrefixLength : ChatMessage.LowerSplit[2].Length)];
            channel.Prefix = prefix;
            Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, "prefix set to: ", prefix);
            return;
        }

        pattern = PatternCreator.Create(_alias, _prefix, @"\semote\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            if (ChatMessage is { IsModerator: false, IsBroadcaster: false })
            {
                Response->Append(ChatMessage.Username, PredefinedMessages.YouArentAModOrTheBroadcaster);
                return;
            }

            string emote = ChatMessage.Split[2][..(ChatMessage.Split[2].Length > AppSettings.MaxEmoteInFrontLength ? AppSettings.MaxEmoteInFrontLength : ChatMessage.Split[2].Length)];
            Channel? channel = _twitchBot.Channels[ChatMessage.ChannelId];
            if (channel is null)
            {
                Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, PredefinedMessages.AnErrorOccurredWhileTryingToSetTheEmote);
                return;
            }

            channel.Emote = emote;
            Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, "emote set to: ", emote);
            return;
        }

        pattern = PatternCreator.Create(_alias, _prefix, @"\s(sr|songrequests?)\s((1|true|enabled?)|(0|false|disabled?))");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace);
            if (ChatMessage is { IsModerator: false, IsBroadcaster: false })
            {
                Response->Append(PredefinedMessages.YouHaveToBeAModOrTheBroadcasterToSetSongRequestSettings);
                return;
            }

            bool? state = null;
            if (_enabledPattern.IsMatch(ChatMessage.Split[2]))
            {
                state = true;
            }
            else if (_disabledPattern.IsMatch(ChatMessage.Split[2]))
            {
                state = false;
            }

            if (state is null)
            {
                Response->Append(PredefinedMessages.TheStateCanOnlyBeSetToEnabledOrDisabled);
                return;
            }

            SpotifyUser? user = _twitchBot.SpotifyUsers[ChatMessage.Channel];
            if (user is null)
            {
                Response->Append("channel ", ChatMessage.Channel, " is not registered, they have to register first");
                return;
            }

            user.AreSongRequestsEnabled = state.Value;
            Response->Append("song requests ", state.Value ? "enabled" : "disabled", "for channel ", ChatMessage.Channel);
            return;
        }

        pattern = PatternCreator.Create(_alias, _prefix, @"\slocation\s((private)|(public))\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            string city = string.Join(' ', ChatMessage.Split, 3, ChatMessage.Split.Length - 3);
            bool isPrivate = ChatMessage.LowerSplit[2][1] == 'r';
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

            Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, "your ", isPrivate ? "private" : "public", " location has been set");
        }
    }
}
