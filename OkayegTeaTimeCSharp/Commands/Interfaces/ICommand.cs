using OkayegTeaTimeCSharp.Twitch.Bot;

namespace OkayegTeaTimeCSharp.Commands.Interfaces
{
    public interface ICommand
    {
        public TwitchBot TwitchBot { get; }

        public ChatMessage ChatMessage { get; }

        public string Alias { get; }

        public void Handle();
    }
}
