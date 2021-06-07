using System;

namespace OkayegTeaTimeCSharp.Utils
{
    public static class NumberHelper
    {
        public static int Random(int min, int max)
        {
            return new Random().Next(min, max);
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

    /// <summary>
    /// Creates a number in which every three digits are devided by a dot.<br/>
    /// For example: 1.465.564
    /// </summary>
    public struct DottedNumber
    {
        public string Number { get; }

        public DottedNumber(long number)
        {
            string num = number.ToString();
            if (num.Length >= 4)
            {
                for (int i = num.Length - 3; i > 0; i -= 3)
                {
                    num = num.Insert(i, ".");
                }
            }
            Number = num;
        }

        public DottedNumber(int number)
        {
            string num = number.ToString();
            if (num.Length >= 4)
            {
                for (int i = num.Length - 3; i > 0; i -= 3)
                {
                    num = num.Insert(i, ".");
                }
            }
            Number = num;
        }

        public override string ToString()
        {
            return Number;
        }
    }
}