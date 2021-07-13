using HLE.Time;
using OkayegTeaTimeCSharp.Commands;
using OkayegTeaTimeCSharp.Commands.CommandEnums;

namespace OkayegTeaTimeCSharp.Twitch.Bot
{
    public class Cooldown
    {
        public string Username { get; set; }

        public CommandType Type { get; set; }

        public long Time { get; private set; }

        public Cooldown(string username, CommandType type)
        {
            Username = username;
            Type = type;
            Time = TimeHelper.Now() + CommandHelper.GetCoolDown(type);
        }
    }
}