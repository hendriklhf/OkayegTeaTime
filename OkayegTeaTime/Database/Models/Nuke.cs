#nullable disable

namespace OkayegTeaTime.Database.Models
{
    public class Nuke
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Channel { get; set; }
        public byte[] Word { get; set; }
        public long TimeoutTime { get; set; }
        public long ForTime { get; set; }

        public const string Reason = "Okayeg nuked word";

        public Nuke(string username, string channel, byte[] word, long timeoutTime, long forTime)
        {
            Username = username;
            Channel = channel;
            Word = word;
            TimeoutTime = timeoutTime;
            ForTime = forTime;
        }
    }
}
