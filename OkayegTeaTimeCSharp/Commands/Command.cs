using OkayegTeaTimeCSharp.Twitch.Bot;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Commands
{
    public abstract class Command
    {
        public TwitchBot TwitchBot { get; }

        public ChatMessage ChatMessage { get; }

        public string Alias { get; }

        public Command(TwitchBot twitchBot, ChatMessage chatMessage, string alias)
        {
            TwitchBot = twitchBot;
            ChatMessage = chatMessage;
            Alias = alias;
        }

        public abstract void Handle();
    }
}
