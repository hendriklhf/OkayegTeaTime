namespace OkayegTeaTimeCSharp.Time
{
    public class Day : ITimeUnit
    {
        public int Count { get; set; }

        public const string Pattern = @"\d+d(ay)?s?";

        private const long InMilliseconds = 86400000;

        public Day(int count = 1)
        {
            Count = count;
        }

        public long ToMilliseconds()
        {
            return Count * InMilliseconds;
        }

        public long ToSeconds()
        {
            return ToMilliseconds() / 1000;
        }
    }
}