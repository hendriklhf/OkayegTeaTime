using HLE;
using HLE.Emojis;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

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
