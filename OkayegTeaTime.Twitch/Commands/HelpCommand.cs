using HLE.Emojis;
using OkayegTeaTime.Files;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Help)]
public class HelpCommand : Command
{
    public HelpCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias) : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        string username = ChatMessage.Split.Length > 1 ? ChatMessage.LowerSplit[1] : ChatMessage.Username;
        Response = $"{Emoji.PointRight} {username}, here you can find a list of commands and the repository: {AppSettings.RepositoryUrl}";
    }
}
