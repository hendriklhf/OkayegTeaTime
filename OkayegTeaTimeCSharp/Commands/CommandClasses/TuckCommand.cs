using OkayegTeaTimeCSharp.Commands.CommandEnums;
using OkayegTeaTimeCSharp.Twitch;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Commands.CommandClasses
{
    public class TuckCommand
    {
        public const CommandType Type = CommandType.Tuck;

        public static void Handle(TwitchBot twitchBot, ChatMessage chatMessage, string alias)
        {
            if (chatMessage.GetMessage().IsMatch(PatternCreator.CreateBoth(alias, @"\s\w+\s\S+")))
            {
                twitchBot.Send(chatMessage.Channel, GenerateMessage(chatMessage.Username, chatMessage.GetLowerSplit()[1], chatMessage.GetSplit()[2]));
            }
            else if (chatMessage.GetMessage().IsMatch(PatternCreator.CreateBoth(alias, @"\s\w+")))
            {
                twitchBot.Send(chatMessage.Channel, GenerateMessage(chatMessage.Username, chatMessage.GetLowerSplit()[1]));
            }
            else
            {
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, who do you want to tuck?");
            }
        }

        private static string GenerateMessage(string username, string target, string emote = "")
        {
            return $"{Emoji.PointRight} {Emoji.Bed} {username} tucked {target} to bed {emote}".Trim();
        }
    }


}
