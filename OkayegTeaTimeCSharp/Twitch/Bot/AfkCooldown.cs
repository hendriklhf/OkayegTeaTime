using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OkayegTeaTimeCSharp.Time;

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
