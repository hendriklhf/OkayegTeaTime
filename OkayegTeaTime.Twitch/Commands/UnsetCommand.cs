using System;
using System.Text.RegularExpressions;
using HLE.Twitch;
using HLE.Twitch.Models;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using Channel = OkayegTeaTime.Database.Models.Channel;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Unset)]
public readonly ref struct UnsetCommand
{
    public ChatMessage ChatMessage { get; }

    private readonly ref MessageBuilder _response;

    private readonly TwitchBot _twitchBot;
    private readonly ReadOnlySpan<char> _prefix;
    private readonly ReadOnlySpan<char> _alias;

    public UnsetCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        ChatMessage = chatMessage;
        _response = ref response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        Regex pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\sprefix");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            UnsetPrefix();
            return;
        }

        pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\sreminder\s\d+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            UnsetReminder();
            return;
        }

        pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\semote");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            UnsetEmote();
            return;
        }

        pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\slocation");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            UnsetLocation();
        }
    }

    private void UnsetPrefix()
    {
        using ChatMessageExtension messageExtension = new(ChatMessage);
        if (!ChatMessage.IsModerator && !messageExtension.IsBroadcaster)
        {
            _response.Append(ChatMessage.Username, ", ", Messages.YouArentAModeratorOrTheBroadcaster);
            return;
        }

        Channel? channel = _twitchBot.Channels[ChatMessage.ChannelId];
        if (channel is null)
        {
            _response.Append(ChatMessage.Username, ", ", Messages.AnErrorOccurredWhileTryingToSetThePrefix);
            return;
        }

        channel.Prefix = null;
        _response.Append(ChatMessage.Username, ", ", Messages.ThePrefixHasBeenUnset);
    }

    private void UnsetReminder()
    {
        using ChatMessageExtension messageExtension = new(ChatMessage);
        _response.Append(ChatMessage.Username, ", ");
        int reminderId = int.Parse(messageExtension.Split[2]);
        bool removed = _twitchBot.Reminders.Remove(ChatMessage.UserId, ChatMessage.Username, reminderId);
        _response.Append(removed ? Messages.TheReminderHasBeenUnset : Messages.TheReminderCouldntBeUnset);
    }

    private void UnsetEmote()
    {
        _response.Append(ChatMessage.Username, ", ");
        using ChatMessageExtension messageExtension = new(ChatMessage);
        if (!ChatMessage.IsModerator && !messageExtension.IsBroadcaster)
        {
            _response.Append(Messages.YouArentAModeratorOrTheBroadcaster);
            return;
        }

        Channel? channel = _twitchBot.Channels[ChatMessage.ChannelId];
        if (channel is null)
        {
            _response.Append(Messages.AnErrorOccurredWhileTryingToSetTheEmote);
            return;
        }

        channel.Emote = null;
        _response.Append(Messages.TheEmoteHasBeenUnset);
    }

    private void UnsetLocation()
    {
        User? user = _twitchBot.Users[ChatMessage.UserId];
        if (user is null)
        {
            _response.Append(ChatMessage.Username, ", ", Messages.YouHaventSetYourLocationYet);
            return;
        }

        user.Location = null;
        _response.Append(ChatMessage.Username, ", ", Messages.YourLocationHasBeenUnset);
    }
}
