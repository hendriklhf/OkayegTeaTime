using System;

namespace OkayegTeaTimeCSharp.Utils
{
    public static class NumberHelper
    {
        public static int Random(int min, int max)
        {
            Random rand = new();
            return rand.Next(min, max);
        }

        public static double ToDouble(this int i)
        {
            return i;
        }

        public static double ToDouble(this long l)
        {
            return l;
        }

        public static long ToLong(this double d)
        {
            return (long)d;
        }
    }
}