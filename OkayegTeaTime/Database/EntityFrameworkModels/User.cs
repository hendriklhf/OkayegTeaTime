using HLE.Time;
using OkayegTeaTime.Twitch.Commands.Enums;

#nullable disable

namespace OkayegTeaTime.Database.EntityFrameworkModels
{
    public class User
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public byte[] AfkMessage { get; set; } = Array.Empty<byte>();
        public int AfkType { get; set; }
        public long AfkTime { get; set; } = TimeHelper.Now();
        public bool? IsAfk { get; set; }

        public User(long id, string username, byte[] afkMessage, int afkType, long afkTime, bool? isAfk)
        {
            Id = id;
            Username = username;
            AfkMessage = afkMessage;
            AfkType = afkType;
            AfkTime = afkTime;
            IsAfk = isAfk;
        }

        public User(long id, string username, AfkCommandType type)
        {
            Id = id;
            Username = username;
            AfkType = (int)type;
            IsAfk = true;
        }
    }
}
