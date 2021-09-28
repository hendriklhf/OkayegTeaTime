namespace OkayegTeaTimeCSharp.Exceptions
{
    public class ReminderNotFoundException : Exception
    {
        public override string Message { get; } = "could not find any matching reminder";

        public ReminderNotFoundException() : base()
        {
        }

        public ReminderNotFoundException(string message) : base(message)
        {
        }
    }
}