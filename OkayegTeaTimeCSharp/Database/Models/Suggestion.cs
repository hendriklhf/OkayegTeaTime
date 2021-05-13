#nullable disable

using OkayegTeaTimeCSharp.Time;

namespace OkayegTeaTimeCSharp.Database.Models
{
    public partial class Suggestion
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public byte[] Suggestion1 { get; set; }
        public string Channel { get; set; }
        public long Time { get; set; } = TimeHelper.Now();
        public bool? Done { get; set; } = false;

        public Suggestion(string username, byte[] suggestion, string channel)
        {
            Username = username;
            Suggestion1 = suggestion;
            Channel = $"#{channel.Replace("#", "")}";
        }
    }
}
