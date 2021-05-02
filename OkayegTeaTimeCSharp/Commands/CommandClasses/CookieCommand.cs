using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OkayegTeaTimeCSharp.Commands.CommandEnums;
using OkayegTeaTimeCSharp.Twitch.Bot;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Commands.CommandClasses
{
    public static class CookieCommand
    {
        public static void Handle(TwitchBot twitchBot, ChatMessage chatMessage, string alias, CommandType type)
        {
            twitchBot.SendRandomCookie(chatMessage);
        }
    }
}
