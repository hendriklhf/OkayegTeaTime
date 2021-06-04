namespace OkayegTeaTimeCSharp.Time
{
    public class Second : ITimeUnit
    {
        public int Count { get; set; }

        public const string Pattern = @"\d+s(ec(ond)?)?s?";

        private const long InMilliseconds = 1000;

        public Second(int count = 1)
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