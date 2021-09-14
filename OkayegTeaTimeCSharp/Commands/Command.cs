using OkayegTeaTimeCSharp.Commands.Interfaces;
using OkayegTeaTimeCSharp.Messages.Interfaces;
using OkayegTeaTimeCSharp.Twitch.Bot;

namespace OkayegTeaTimeCSharp.Commands
{
    public abstract class Command : ICommand
    {
        public TwitchBot TwitchBot { get; }

        public ITwitchChatMessage ChatMessage { get; }

        public string Alias { get; }

        public Command(TwitchBot twitchBot, ITwitchChatMessage chatMessage, string alias)
        {
            TwitchBot = twitchBot;
            ChatMessage = chatMessage;
            Alias = alias;
        }

        public abstract void Handle();
    }
}
