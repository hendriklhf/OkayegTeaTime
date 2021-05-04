using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OkayegTeaTimeCSharp.Twitch.Bot;
using TwitchLib.Client.Models;
using OkayegTeaTimeCSharp.Utils;
using OkayegTeaTimeCSharp.Twitch;

namespace OkayegTeaTimeCSharp.Commands.CommandClasses
{
    public class TuckCommand
    {
        public static void Handle(TwitchBot twitchBot, ChatMessage chatMessage)
        {
            if (chatMessage.GetSplit().Length >= 3)
            {
                twitchBot.Send(chatMessage.Channel, GenerateMessage(chatMessage.Username, chatMessage.GetLowerSplit()[1], chatMessage.GetSplit()[2]));
            }
            else if (chatMessage.GetSplit().Length >= 2)
            {
                twitchBot.Send(chatMessage.Channel, GenerateMessage(chatMessage.Username, chatMessage.GetLowerSplit()[1]));
            }
        }

        private static string GenerateMessage(string username, string target, string emote = "")
        {
            return $"{Emoji.PointRight} {Emoji.Bed} {username} tucked {target} to bed {emote}".Trim();
        }
    }


}
