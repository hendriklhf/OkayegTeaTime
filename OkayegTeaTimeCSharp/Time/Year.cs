namespace OkayegTeaTimeCSharp.Time
{
    public static class Year
    {
        private const long InMilliseconds = 31556952000;
        public const string Pattern = @"\d+y(ear)?s?";

        public static long ToMilliseconds(uint years = 1)
        {
            return InMilliseconds * years;
        }
    }
}
