using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkayegTeaTimeCSharp.Exceptions
{
    public class MessageNotFoundException : Exception
    {
        public override string Message { get; } = "could not find any matching message";

        public MessageNotFoundException() : base()
        {
        }

        public MessageNotFoundException(string message) : base(message)
        {
            Message = message;
        }
    }
}
