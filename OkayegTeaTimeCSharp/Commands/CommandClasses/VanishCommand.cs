using OkayegTeaTimeCSharp.Messages;
using OkayegTeaTimeCSharp.Twitch.Bot;
using System;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;


namespace OkayegTeaTimeCSharp.Commands.CommandClasses
{
    public static class VanishCommand
    {
        public static void Handle(TwitchBot twitchBot, ChatMessage chatMessage, string alias)
        {
            if (!chatMessage.IsModOrBroadcaster())
            {
                twitchBot.TwitchClient.TimeoutUser(chatMessage.Channel, chatMessage.Username, TimeSpan.FromSeconds(1));
            }
        }
    }
}