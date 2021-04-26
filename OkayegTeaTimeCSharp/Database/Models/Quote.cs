#nullable disable

namespace OkayegTeaTimeCSharp.Database.Models
{
    public partial class Quote
    {
        public int Id { get; set; }
        public byte[] QuoteMessage { get; set; }
        public string Submitter { get; set; }
        public string TargetUser { get; set; }
        public string Channel { get; set; }
    }
}
