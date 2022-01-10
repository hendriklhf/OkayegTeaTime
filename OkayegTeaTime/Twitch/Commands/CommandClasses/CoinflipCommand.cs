using HLE.Emojis;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;
using Random = HLE.Random.Random;

namespace OkayegTeaTime.Twitch.Commands.CommandClasses;

public class CoinflipCommand : Command
{
    public CoinflipCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        string result = Random.Bool() ? "yes/heads" : "no/tails";
        Response = $"{ChatMessage.Username}, {result} {Emoji.Coin}";
    }
}
