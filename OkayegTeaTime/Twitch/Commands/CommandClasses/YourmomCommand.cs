using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands.CommandClasses;

public class YourmomCommand : Command
{
    public YourmomCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Yourmom? yourmom = DbController.GetRandomYourmom();
        string target = ChatMessage.LowerSplit.Length > 1 ? ChatMessage.LowerSplit[1] : ChatMessage.Username;
        if (yourmom is null)
        {
            Response = "couldn't find a joke";
            return;
        }
        Response = $"{target}, {yourmom.MessageText} YOURMOM";
        return;
    }
}
