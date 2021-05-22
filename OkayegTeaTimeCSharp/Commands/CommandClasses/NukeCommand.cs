using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Twitch;
using OkayegTeaTimeCSharp.Utils;
using TwitchLib.Client.Models;
using OkayegTeaTimeCSharp.Time;

namespace OkayegTeaTimeCSharp.Commands.CommandClasses
{
    public class NukeCommand
    {
        public static void Handle(TwitchBot twitchBot, ChatMessage chatMessage, string alias)
        {
            if (chatMessage.GetMessage().IsMatch(PatternCreator.CreateBoth(alias, @"\s\S+\s" + Pattern.TimeSplitPattern + @"\s" + Pattern.TimeSplitPattern + @"(\s|$)")))
            {
                List<string> timeoutTime = new() { chatMessage.GetLowerSplit()[2] };
                List<string> duration = new() { chatMessage.GetLowerSplit()[3] };

                twitchBot.SendCreatedNuke(chatMessage, chatMessage.GetLowerSplit()[1], TimeHelper.ConvertStringToSeconds(timeoutTime), TimeHelper.ConvertStringToMilliseconds(duration));
            }
        }
    }
}
