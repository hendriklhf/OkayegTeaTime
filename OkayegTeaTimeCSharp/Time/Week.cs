namespace OkayegTeaTimeCSharp.Time
{
    public class Week : ITimeUnit
    {
        public int Count { get; set; }

        public const string Pattern = @"\d+w(eek)?s?";

        public Week(int count = 1)
        {
            Count = count;
        }

        public long ToMilliseconds()
        {
            return new Day(7 * Count).ToMilliseconds();
        }

        public long ToSeconds()
        {
            return ToMilliseconds() / 1000;
        }
    }
}
