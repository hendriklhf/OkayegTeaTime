using OkayegTeaTimeCSharp.Twitch.Bot;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Commands
{
    public interface ICommand
    {
        public void Handle(TwitchBot twitchBot, ChatMessage chatMessage, string alias);
    }
}
