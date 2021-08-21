using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Twitch.Interfaces;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Twitch
{
    public abstract class Handler : IHandler
    {
        public TwitchBot TwitchBot { get; }

        public ChatMessage ChatMessage { get; }

        public Handler(TwitchBot twitchBot, ChatMessage chatMessage)
        {
            TwitchBot = twitchBot;
            ChatMessage = chatMessage;
        }

        public abstract void Handle();
    }
}
