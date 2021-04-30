using System.Collections.Generic;
using System.Linq;

namespace OkayegTeaTimeCSharp.JsonData
{
    public static class DataHelper
    {
        public static string GetDiscordID(string username)
        {
            return JsonHelper.BotData.Discord.UserList.Where(user => user.Name == username).FirstOrDefault().Id;
        }

        public static List<string> GetDiscordMatches(string username)
        {
            return JsonHelper.BotData.Discord.UserList.Where(user => user.Name == username).FirstOrDefault().Matches;
        }
    }
}
