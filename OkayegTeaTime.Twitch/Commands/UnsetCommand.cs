using System.Text.RegularExpressions;
using HLE;
using HLE.Twitch.Models;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using Channel = OkayegTeaTime.Database.Models.Channel;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Unset)]
public readonly unsafe ref struct UnsetCommand
{
    public ChatMessage ChatMessage { get; }

    public StringBuilder* Response { get; }

    private readonly TwitchBot _twitchBot;
    private readonly string? _prefix;
    private readonly string _alias;

    public UnsetCommand(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        ChatMessage = chatMessage;
        Response = response;
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
            Response->Append(ChatMessage.Username, Messages.CommaSpace, Messages.YouArentAModeratorOrTheBroadcaster);
            return;
        }

        Channel? channel = _twitchBot.Channels[ChatMessage.ChannelId];
        if (channel is null)
        {
            Response->Append(ChatMessage.Username, Messages.CommaSpace, Messages.AnErrorOccurredWhileTryingToSetThePrefix);
            return;
        }

        channel.Prefix = null;
        Response->Append(ChatMessage.Username, Messages.CommaSpace, Messages.ThePrefixHasBeenUnset);
    }

    private void UnsetReminder()
    {
        using ChatMessageExtension messageExtension = new(ChatMessage);
        Response->Append(ChatMessage.Username, Messages.CommaSpace);
        int reminderId = int.Parse(messageExtension.Split[2]);
        bool removed = _twitchBot.Reminders.Remove(ChatMessage.UserId, ChatMessage.Username, reminderId);
        Response->Append(removed ? Messages.TheReminderHasBeenUnset : Messages.TheReminderCouldntBeUnset);
    }

    private void UnsetEmote()
    {
        Response->Append(ChatMessage.Username, Messages.CommaSpace);
        using ChatMessageExtension messageExtension = new(ChatMessage);
        if (!ChatMessage.IsModerator && !messageExtension.IsBroadcaster)
        {
            Response->Append(Messages.YouArentAModeratorOrTheBroadcaster);
            return;
        }

        Channel? channel = _twitchBot.Channels[ChatMessage.ChannelId];
        if (channel is null)
        {
            Response->Append(Messages.AnErrorOccurredWhileTryingToSetTheEmote);
            return;
        }

        channel.Emote = null;
        Response->Append(Messages.TheEmoteHasBeenUnset);
    }

    private void UnsetLocation()
    {
        User? user = _twitchBot.Users[ChatMessage.UserId];
        if (user is null)
        {
            Response->Append(ChatMessage.Username, Messages.CommaSpace, Messages.YouHaventSetYourLocationYet);
            return;
        }

        user.Location = null;
        Response->Append(ChatMessage.Username, Messages.CommaSpace, Messages.YourLocationHasBeenUnset);
    }
}
