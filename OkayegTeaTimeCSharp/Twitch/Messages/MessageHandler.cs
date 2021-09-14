using OkayegTeaTimeCSharp.Commands;
using OkayegTeaTimeCSharp.Database;
using OkayegTeaTimeCSharp.Handlers;
using OkayegTeaTimeCSharp.Messages.Enums;
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
            if (!chatMessage.UserTags.Contains(UserTag.Special))
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
