using OkayegTeaTimeCSharp.Commands;
using OkayegTeaTimeCSharp.Database;
using OkayegTeaTimeCSharp.Discord;
using OkayegTeaTimeCSharp.Twitch.Bot;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Messages
{
    public static class MessageHandler
    {
        public static void Handle(TwitchBot twitchBot, ChatMessage chatMessage)
        {
            if (!MessageHelper.IsSpecialUser(chatMessage.Username))
            {
                DataBase.LogMessage(chatMessage);

                DataBase.CheckIfAFK(twitchBot, chatMessage);

                DataBase.CheckForReminder(twitchBot, chatMessage);

                DataBase.InsertNewUser(chatMessage.Username);

                CommandHandler.Handle(twitchBot, chatMessage);

                DataBase.CheckForNukes(twitchBot, chatMessage);

                DiscordClient.SendDiscordMessageIfAFK(twitchBot, chatMessage);
            }
        }
    }
}
