namespace OkayegTeaTimeCSharp.Time
{
    public static class Second
    {
        private const long InMilliseconds = 1000;
        public const string Pattern = @"\d+s(ec(ond)?)?s?";

        public static long ToMilliseconds(uint seconds = 1)
        {
            return InMilliseconds * seconds;
        }
    }
}
