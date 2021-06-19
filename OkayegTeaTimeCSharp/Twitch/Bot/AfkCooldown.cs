using Sterbehilfe.Time;

namespace OkayegTeaTimeCSharp.Twitch.Bot
{
    public class AfkCooldown
    {
        public string Username { get; set; }

        public long Time { get; private set; }

        public AfkCooldown(string username)
        {
            Username = username;
            Time = TimeHelper.Now() + Config.AfkCooldown;
        }
    }
}