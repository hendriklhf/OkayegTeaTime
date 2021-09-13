using HLE.Strings;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;

namespace OkayegTeaTimeCSharp.Commands.CommandClasses
{
    public class SpotifyCommand : Command
    {
        public SpotifyCommand(TwitchBot twitchBot, ChatMessage chatMessage, string alias)
            : base(twitchBot, chatMessage, alias)
        {
        }

        public override void Handle()
        {
            if (ChatMessage.GetMessage().IsMatch(PatternCreator.Create(Alias, PrefixDictionary.Get(ChatMessage.Channel), @"\ssearch\s.+")))
            {
                TwitchBot.Send(ChatMessage.Channel, BotActions.SendSpotifySearch(ChatMessage));
            }
            else if (ChatMessage.GetMessage().IsMatch(PatternCreator.Create(Alias, PrefixDictionary.Get(ChatMessage.Channel), @"(\s\w+)?")))
            {
                TwitchBot.Send(ChatMessage.Channel, BotActions.SendSpotifyCurrentlyPlaying(ChatMessage));
            }
        }
    }
}
