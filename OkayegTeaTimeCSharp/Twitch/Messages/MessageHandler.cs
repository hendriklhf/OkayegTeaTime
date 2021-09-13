using OkayegTeaTimeCSharp.Database;
using OkayegTeaTimeCSharp.Messages.Interfaces;
using OkayegTeaTimeCSharp.Twitch.Bot;

namespace OkayegTeaTimeCSharp.Twitch.Messages
{
    public class MessageHandler : Handler
    {
        public CommandHandler CommandHandler { get; }

        public MessageHandler(TwitchBot twitchBot)
            : base(twitchBot)
        {
            CommandHandler = new(twitchBot);
        }

        public override void Handle(ITwitchChatMessage chatMessage)
        {
            if (!chatMessage.Username.IsSpecialUser())
            {
                DataBase.InsertNewUser(chatMessage.Username);

                DataBase.LogMessage(chatMessage);

                DataBase.CheckIfAFK(TwitchBot, chatMessage);

                DataBase.CheckForReminder(TwitchBot, chatMessage);

                CommandHandler.Handle(chatMessage);

                DataBase.CheckForNukes(TwitchBot, chatMessage);
            }
        }
    }
}
