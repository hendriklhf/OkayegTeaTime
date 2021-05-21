using System;

namespace OkayegTeaTimeCSharp.Exceptions
{
    public class ChannelNotFoundException : Exception
    {
        public override string Message { get; } = "could not find any matching channel";

        public ChannelNotFoundException() : base()
        {
        }

        public ChannelNotFoundException(string message) : base(message)
        {
            Message = message;
        }
    }
}