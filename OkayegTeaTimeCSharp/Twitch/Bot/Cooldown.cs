using HLE.Time;
using OkayegTeaTimeCSharp.Commands;
using OkayegTeaTimeCSharp.Commands.Enums;

namespace OkayegTeaTimeCSharp.Twitch.Bot;

public class Cooldown
{
    public string Username { get; }

    public CommandType Type { get; }

    public long Time { get; private set; }

    public Cooldown(string username, CommandType type)
    {
        Username = username;
        Type = type;
        Time = TimeHelper.Now() + CommandHelper.GetCoolDown(type);
    }
}
