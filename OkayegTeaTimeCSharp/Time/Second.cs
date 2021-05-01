namespace OkayegTeaTimeCSharp.Time
{
    public static class Second
    {
        private const long InMilliseconds = 1000;

        public static long ToMilliseconds(uint seconds = 1)
        {
            return InMilliseconds * seconds;
        }
    }
}
