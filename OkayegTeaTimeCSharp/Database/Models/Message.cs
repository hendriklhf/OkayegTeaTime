#nullable disable

using OkayegTeaTimeCSharp.Time;

namespace OkayegTeaTimeCSharp.Database.Models
{
    public partial class Message
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
            Channel = channel;
        }
    }
}
