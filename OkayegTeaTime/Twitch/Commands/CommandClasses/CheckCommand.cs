using System.Text.RegularExpressions;
using HLE.Strings;
using HLE.Time;
using HLE.Time.Enums;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Commands.AfkCommandClasses;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands.CommandClasses;

public class CheckCommand : Command
{
    public CheckCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex afkPattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\safk\s\w+");
        if (afkPattern.IsMatch(ChatMessage.Message))
        {
            Response = $"{ChatMessage.Username}, ";
            string username = ChatMessage.LowerSplit[2];
            User? user = DbController.GetUser(username);
            if (user is null)
            {
                Response += PredefinedMessages.UserNotFoundMessage;
                return;
            }

            if (user.IsAfk == true)
            {
                Response += new AfkMessage(user).GoingAway;
                string message = user.MessageText.Decode();
                if (!string.IsNullOrEmpty(message))
                {
                    Response += $": {user.MessageText.Decode()}";
                }
                Response += $" ({TimeHelper.ConvertUnixTimeToTimeStamp(user.Time, "ago", ConversionType.YearDayHourMin)})";
            }
            else
            {
                Response += $"{username} is not afk";
            }
            return;
        }

        Regex reminderPattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\sreminder\s\d+");
        if (reminderPattern.IsMatch(ChatMessage.Message))
        {
            Response = $"{ChatMessage.Username}, ";
            int id = ChatMessage.Split[2].ToInt();
            Reminder? reminder = DbController.GetReminder(id);
            if (reminder is null)
            {
                Response += PredefinedMessages.ReminderNotFoundMessage;
                return;
            }

            Response += $"From: {reminder.GetAuthor()} || To: {reminder.GetTarget()} || ";
            Response += $"Set: {TimeHelper.ConvertUnixTimeToTimeStamp(reminder.Time, "ago", ConversionType.YearDayHourMin)} || ";
            Response += reminder.ToTime > 0 ? $"Fires in: {TimeHelper.ConvertUnixTimeToTimeStamp(reminder.ToTime, conversionType: ConversionType.YearDayHourMin)} || " : string.Empty;
            Response += $"Message: {reminder.Message.Decode()}";
            return;
        }
    }
}
