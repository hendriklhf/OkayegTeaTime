using System.Reflection;
using OkayegTeaTime.Twitch.Commands;

namespace OkayegTeaTime.Twitch.Models;

public sealed class CommandHandle
{
    private readonly ConstructorInfo _constructor;

    public CommandHandle(ConstructorInfo constructor)
    {
        _constructor = constructor;
    }

    public Command CreateCommandInstance(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
    {
        return (Command)_constructor.Invoke(new object[]
        {
            twitchBot,
            chatMessage,
            alias
        });
    }
}
