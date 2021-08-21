using OkayegTeaTimeCSharp.Database;
using OkayegTeaTimeCSharp.Discord;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Twitch.Commands;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Twitch.Messages
{
    public class MessageHandler : Handler
    {
        public CommandHandler CommandHandler { get; }

        public MessageHandler(TwitchBot twitchBot, ChatMessage chatMessage)
            : base(twitchBot, chatMessage)
        {
            CommandHandler = new(twitchBot, chatMessage);
        }

        public override void Handle()
        {
            if (!ChatMessage.Username.IsSpecialUser())
            {
                DataBase.InsertNewUser(ChatMessage.Username);

                DataBase.LogMessage(ChatMessage);

                DataBase.CheckIfAFK(TwitchBot, ChatMessage);

                DataBase.CheckForReminder(TwitchBot, ChatMessage);

                CommandHandler.Handle();

                DataBase.CheckForNukes(TwitchBot, ChatMessage);

                DiscordClient.SendDiscordMessageIfAFK(TwitchBot, ChatMessage);
            }
        }
    }
}