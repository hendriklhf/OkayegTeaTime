namespace OkayegTeaTimeCSharp.Time
{
    public static class Day
    {
        private const long InMilliseconds = 86400000;
        public const string Pattern = @"\d+d(ay)?s?";

        public static long ToMilliseconds(uint days = 1)
        {
            return InMilliseconds * days;
        }

        public static long ToSeconds(uint days = 1)
        {
            return ToMilliseconds(days) / 1000;
        }
    }
}