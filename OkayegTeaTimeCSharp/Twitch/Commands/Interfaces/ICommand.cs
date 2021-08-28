using OkayegTeaTimeCSharp.Twitch.Bot;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Twitch.Commands.Interfaces
{
    public interface ICommand
    {
        public TwitchBot TwitchBot { get; }

        public ChatMessage ChatMessage { get; }

        public string Alias { get; }

        public void Handle();
    }
}
