using HLE.Time;

namespace OkayegTeaTimeCSharp.Twitch.Bot
{
    public class AfkCooldown
    {
        public string Username { get; }

        public long Time { get; private set; }

        public AfkCooldown(string username)
        {
            Username = username;
            Time = TimeHelper.Now() + Config.AfkCooldown;
        }
    }
}
