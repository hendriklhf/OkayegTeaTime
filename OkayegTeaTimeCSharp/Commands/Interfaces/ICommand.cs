using OkayegTeaTimeCSharp.Messages.Interfaces;
using OkayegTeaTimeCSharp.Twitch.Bot;

namespace OkayegTeaTimeCSharp.Commands.Interfaces
{
    public interface ICommand
    {
        public TwitchBot TwitchBot { get; }

        public ITwitchChatMessage ChatMessage { get; }

        public string Alias { get; }

        public void Handle();
    }
}
