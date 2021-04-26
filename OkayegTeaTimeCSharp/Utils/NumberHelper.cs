namespace OkayegTeaTimeCSharp.Utils
{
    public static class NumberHelper
    {
        public static long ToLong(this double d)
        {
            return (long)d;
        }

        public static double ToDouble(this int i)
        {
            return i;
        }
    }
}
