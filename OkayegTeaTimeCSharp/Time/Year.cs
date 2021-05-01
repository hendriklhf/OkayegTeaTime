namespace OkayegTeaTimeCSharp.Time
{
    public static class Year
    {
        private const long InMilliseconds = 31556952000;

        public static long ToMilliseconds(uint seconds = 1)
        {
            return InMilliseconds * seconds;
        }
    }
}
