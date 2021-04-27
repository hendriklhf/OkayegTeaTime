namespace OkayegTeaTimeCSharp.Time
{
    public static class Day
    {
        private const long InMilliseconds = 86400000;

        public static long ToMilliseconds(int days = 1)
        {
            return InMilliseconds * days;
        }
    }
}
