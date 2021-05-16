namespace OkayegTeaTimeCSharp.Time
{
    public static class Hour
    {
        private const long InMilliseconds = 3600000;
        public const string Pattern = @"\d+h(our)?s?";

        public static long ToMilliseconds(uint hours = 1)
        {
            return InMilliseconds * hours;
        }
    }
}
