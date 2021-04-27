namespace OkayegTeaTimeCSharp.Time
{
    public static class Hour
    {
        private const long InMilliseconds = 3600000;

        public static long ToMilliseconds(int hours = 1)
        {
            return InMilliseconds * hours;
        }
    }
}
