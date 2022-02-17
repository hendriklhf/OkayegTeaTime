using System.Text.RegularExpressions;
using HLE.Strings;
using HLE.Time;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Twitch.Api;
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
            int? userId = TwitchApi.GetUserId(username);
            if (userId is null)
            {
                Response += PredefinedMessages.UserNotFoundMessage;
                return;
            }

            User? user = DbController.GetUser(userId.Value, username);
            if (user is null)
            {
                Response += PredefinedMessages.UserNotFoundMessage;
                return;
            }

            if (user.IsAfk == true)
            {
                Response += new AfkMessage(userId.Value).GoingAway!;
                string message = user.AfkMessage.Decode();
                if (!string.IsNullOrEmpty(message))
                {
                    Response += $": {message}";
                }
                Response += $" ({TimeHelper.GetUnixDifference(user.AfkTime)} ago)";
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
            Response += $"Set: {TimeHelper.GetUnixDifference(reminder.Time)} ago || ";
            Response += reminder.ToTime > 0 ? $"Fires in: {TimeHelper.GetUnixDifference(reminder.ToTime)} || " : string.Empty;
            Response += $"Message: {reminder.Message.Decode()}";
            return;
        }
    }
}
