namespace OkayegTeaTimeCSharp.Time
{
    public static class Minute
    {
        private const long InMilliseconds = 60000;
        public const string Pattern = @"\d+m(in(ute)?)?s?";

        public static long ToMilliseconds(uint minutes = 1)
        {
            return InMilliseconds * minutes;
        }
    }
}
