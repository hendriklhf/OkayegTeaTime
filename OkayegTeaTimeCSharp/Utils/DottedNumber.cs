namespace OkayegTeaTimeCSharp.Utils
{
    /// <summary>
    /// Creates a number in which every three digits are devided by a dot.<br/>
    /// For example: 1.465.564
    /// </summary>
    public struct DottedNumber
    {
        /// <summary>
        /// The number with the dots inserted.
        /// </summary>
        public string Number { get; }

        /// <summary>
        /// The original number passed to the constructor.
        /// </summary>
        public long OrigninalNumber { get; }

        /// <summary>
        /// The basic constructor of DottedNumber.
        /// </summary>
        /// <param name="number">A number of type long in which the dots will be inserted</param>
        public DottedNumber(long number)
        {
            OrigninalNumber = number;
            string num = OrigninalNumber.ToString();
            if (num.Length >= 4)
            {
                for (int i = num.Length - 3; i > 0; i -= 3)
                {
                    num = num.Insert(i, ".");
                }
            }
            Number = num;
        }

        public static bool operator >(DottedNumber left, DottedNumber right)
        {
            return left.OrigninalNumber > right.OrigninalNumber;
        }

        public static bool operator <(DottedNumber left, DottedNumber right)
        {
            return left.OrigninalNumber < right.OrigninalNumber;
        }

        public static bool operator >=(DottedNumber left, DottedNumber right)
        {
            return left.OrigninalNumber >= right.OrigninalNumber;
        }

        public static bool operator <=(DottedNumber left, DottedNumber right)
        {
            return left.OrigninalNumber <= right.OrigninalNumber;
        }

        public static DottedNumber operator +(DottedNumber left, DottedNumber right)
        {
            return new(left.OrigninalNumber + right.OrigninalNumber);
        }

        public static DottedNumber operator -(DottedNumber left, DottedNumber right)
        {
            return new(left.OrigninalNumber - right.OrigninalNumber);
        }

        public static DottedNumber operator *(DottedNumber left, DottedNumber right)
        {
            return new(left.OrigninalNumber * right.OrigninalNumber);
        }

        public static DottedNumber operator /(DottedNumber left, DottedNumber right)
        {
            return new(left.OrigninalNumber / right.OrigninalNumber);
        }

        public static DottedNumber operator ++(DottedNumber dottedNumber)
        {
            return new(dottedNumber.OrigninalNumber + 1);
        }

        public static DottedNumber operator --(DottedNumber dottedNumber)
        {
            return new(dottedNumber.OrigninalNumber - 1);
        }

        public override string ToString()
        {
            return Number;
        }
    }
}
