namespace OkayegTeaTimeCSharp.Time
{
    public class Hour : ITimeUnit
    {
        public int Count { get; set; }

        public const string Pattern = @"\d+h(our)?s?";

        private const long _inMilliseconds = 3600000;

        public Hour(int count = 1)
        {
            Count = count;
        }

        public long ToMilliseconds()
        {
            return Count * _inMilliseconds;
        }

        public long ToSeconds()
        {
            return ToMilliseconds() / 1000;
        }
    }
}