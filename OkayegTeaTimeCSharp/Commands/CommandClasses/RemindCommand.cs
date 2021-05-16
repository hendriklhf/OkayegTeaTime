using OkayegTeaTimeCSharp.Twitch;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Commands.CommandClasses
{
    public static class RemindCommand
    {
        private const string ReminderInTimePattern = @"\s\w+\sin\s(" + TimeSplitPattern + @"\s)+(\S|\s)+";
        private const string TimeSplitPattern = @"(\d+(y(ear)?|d(ay)?|h(our)?|m(in(ute)?)?|s(ecs(ond)?)?)s?)";

        public static void Handle(TwitchBot twitchBot, ChatMessage chatMessage, string alias)
        {
            if (chatMessage.GetMessage().IsMatch(PatternCreator.CreateBoth(alias, ReminderInTimePattern)))
            {

            }
            else if (chatMessage.GetMessage().IsMatch(PatternCreator.CreateBoth(alias, @"\s\w+\s\S+")))
            {

            }
        }
    }
}
