#nullable disable

using HLE.Time;

namespace OkayegTeaTime.Database.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public byte[] MessageText { get; set; }
        public string Channel { get; set; }
        public long Time { get; set; } = TimeHelper.Now();

        public Message(string username, byte[] messageText, string channel)
        {
            Username = username;
            MessageText = messageText;
            Channel = $"#{channel.RemoveHashtag()}";
        }
    }
}