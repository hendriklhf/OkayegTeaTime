namespace OkayegTeaTimeCSharp.Time
{
    public class Year : ITimeUnit
    {
        public int Count { get; set; }

        public const string Pattern = @"\d+y(ear)?s?";

        private const long InMilliseconds = 31556952000;

        public Year(int count = 1)
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