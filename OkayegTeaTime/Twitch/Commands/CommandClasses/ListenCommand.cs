using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OkayegTeaTime.Spotify;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands.CommandClasses;

public class ListenCommand : Command
{
    public ListenCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex pattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s\w+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            string response = Task.Run(async () => await SpotifyRequest.ListenTo(ChatMessage.Username, ChatMessage.LowerSplit[1])).Result;
            Response = $"{ChatMessage.Username}, {response}";
            return;
        }
    }
}
