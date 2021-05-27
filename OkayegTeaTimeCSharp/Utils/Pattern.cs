namespace OkayegTeaTimeCSharp.Utils
{
    public class Pattern
    {
        public const string ReminderInTimePattern = @"\s\w+\sin\s(" + TimeSplitPattern + @"\s)+(\S|\s)+";
        public const string TimeSplitPattern = @"(\d+(y(ear)?|d(ay)?|h(our)?|m(in(ute)?)?|s(ecs(ond)?)?)s?)";
    }
}