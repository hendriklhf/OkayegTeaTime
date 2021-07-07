using System;

namespace OkayegTeaTimeCSharp.Exceptions
{
    public class NukeNotFoundException : Exception
    {
        public override string Message { get; } = "could not find any matching nuke";

        public NukeNotFoundException() : base()
        {
        }

        public NukeNotFoundException(string message) : base(message)
        {
        }
    }
}
