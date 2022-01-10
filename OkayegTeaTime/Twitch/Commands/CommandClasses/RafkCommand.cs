using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Commands.AfkCommandClasses;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands.CommandClasses;

public class RafkCommand : Command
{
    public RafkCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        User? user = DbController.GetUser(ChatMessage.Username);
        if (user is null)
        {
            DbController.AddUser(ChatMessage.Username);
            user = DbController.GetUser(ChatMessage.Username);
        }
        if (string.IsNullOrEmpty(user!.Type))
        {
            Response = $"{ChatMessage.Username}, can't resume your afk status, because you never went afk before";
            return;
        }

        DbController.ResumeAfkStatus(ChatMessage.Username);
        Response = new AfkMessage(user!).Resuming;
    }
}
