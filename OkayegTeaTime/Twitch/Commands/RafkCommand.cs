using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

public class RafkCommand : Command
{
    public RafkCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        User? user = DbControl.Users.GetUser(ChatMessage.UserId, ChatMessage.Username);
        if (user is null)
        {
            Response = $"{ChatMessage.Username}, can't resume your afk status, because you never went afk before";
            return;
        }

        user.IsAfk = true;
        Response = new AfkMessage(user.Id).Resuming!;
    }
}
