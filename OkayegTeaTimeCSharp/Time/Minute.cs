namespace OkayegTeaTimeCSharp.Time
{
    public class Minute : ITimeUnit
    {
        public int Count { get; set; }

        public const string Pattern = @"\d+m(in(ute)?)?s?";

        private const long InMilliseconds = 60000;

        public Minute(int count = 1)
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