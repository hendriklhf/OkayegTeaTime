using OkayegTeaTimeCSharp.Messages;
using OkayegTeaTimeCSharp.Twitch.Bot;
using System;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Commands.CommandClasses
{
    public class VanishCommand : Command
    {
        public VanishCommand(TwitchBot twitchBot, ChatMessage chatMessage, string alias)
            : base(twitchBot, chatMessage, alias)
        {
        }

        public override void Handle()
        {
            if (!ChatMessage.IsModOrBroadcaster())
            {
                TwitchBot.TwitchClient.TimeoutUser(ChatMessage.Channel, ChatMessage.Username, TimeSpan.FromSeconds(1));
            }
        }
    }
}