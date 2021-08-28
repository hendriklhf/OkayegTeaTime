#nullable disable

namespace OkayegTeaTimeCSharp.Database.Models
{
    public partial class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public byte[] MessageText { get; set; }
        public string Type { get; set; }
        public long Time { get; set; } = 0;
        public string IsAfk { get; set; } = "False";

        public User(int id, string username, byte[] messageText, string type, long time, string isAfk)
        {
            Id = id;
            Username = username;
            MessageText = messageText;
            Type = type;
            Time = time;
            IsAfk = isAfk;
        }

        public User(string username)
        {
            Username = username;
        }
    }
}
