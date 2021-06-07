namespace OkayegTeaTimeCSharp.Utils
{
    /// <summary>
    /// Creates a number in which every three digits are devided by a dot.<br/>
    /// For example: 1.465.564
    /// </summary>
    public struct DottedNumber
    {
        public string Number { get; }

        public long OrigninalNumber { get; }

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
            OrigninalNumber = number;
        }

        public override string ToString() => Number;
    }
}
