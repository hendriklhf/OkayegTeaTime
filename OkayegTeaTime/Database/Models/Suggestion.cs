using HLE.Strings;
using HLE.Time;

#nullable disable

namespace OkayegTeaTime.Database.Models
{
    public class Suggestion
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public byte[] Suggestion1 { get; set; }
        public string Channel { get; set; }
        public long Time { get; set; } = TimeHelper.Now();
        public string Status { get; set; } = "Open";

        public Suggestion(int id, string username, byte[] suggestion1, string channel, long time, string status)
        {
            Id = id;
            Username = username;
            Suggestion1 = suggestion1;
            Channel = channel;
            Time = time;
            Status = status;
        }

        public Suggestion(string username, byte[] suggestion, string channel)
        {
            Username = username;
            Suggestion1 = suggestion;
            Channel = $"#{channel.Remove("#")}";
        }
    }
}
