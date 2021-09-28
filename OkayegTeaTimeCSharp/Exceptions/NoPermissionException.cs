namespace OkayegTeaTimeCSharp.Exceptions
{
    public class NoPermissionException : Exception
    {
        public override string Message { get; } = "you have no permission to perform this action";

        public NoPermissionException()
        {
        }

        public NoPermissionException(string message) : base(message)
        {
        }
    }
}
