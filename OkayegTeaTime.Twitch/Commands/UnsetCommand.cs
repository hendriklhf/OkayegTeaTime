﻿using System.Text.RegularExpressions;
using HLE;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Unset)]
public readonly unsafe ref struct UnsetCommand
{
    public TwitchChatMessage ChatMessage { get; }

    public StringBuilder* Response { get; }

    private readonly TwitchBot _twitchBot;
    private readonly string? _prefix;
    private readonly string _alias;

    public UnsetCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
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
            if (ChatMessage.IsModerator || ChatMessage.IsBroadcaster)
            {
                Channel? channel = _twitchBot.Channels[ChatMessage.ChannelId];
                if (channel is null)
                {
                    Response->Append(ChatMessage.Username, Messages.CommaSpace, Messages.AnErrorOccurredWhileTryingToSetThePrefix);
                    return;
                }

                channel.Prefix = null;
                Response->Append(ChatMessage.Username, Messages.CommaSpace, Messages.ThePrefixHasBeenUnset);
            }
            else
            {
                Response->Append(ChatMessage.Username, Messages.CommaSpace, Messages.YouArentAModOrTheBroadcaster);
            }

            return;
        }

        pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\sreminder\s\d+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Response->Append(ChatMessage.Username, Messages.CommaSpace);
            int reminderId = int.Parse(ChatMessage.Split[2]);
            bool removed = _twitchBot.Reminders.Remove(ChatMessage.UserId, ChatMessage.Username, reminderId);
            Response->Append(removed ? Messages.TheReminderHasBeenUnset : Messages.TheReminderCouldntBeUnset);
            return;
        }

        pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\semote");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Response->Append(ChatMessage.Username, Messages.CommaSpace);
            if (ChatMessage.IsModerator || ChatMessage.IsBroadcaster)
            {
                Channel? channel = _twitchBot.Channels[ChatMessage.ChannelId];
                if (channel is null)
                {
                    Response->Append(Messages.AnErrorOccurredWhileTryingToSetTheEmote);
                    return;
                }

                channel.Emote = null;
                Response->Append(Messages.TheEmoteHasBeenUnset);
            }
            else
            {
                Response->Append(Messages.YouArentAModOrTheBroadcaster);
            }

            return;
        }

        pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\slocation");
        if (pattern.IsMatch(ChatMessage.Message))
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
}
