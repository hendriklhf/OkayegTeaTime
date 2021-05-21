using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkayegTeaTimeCSharp.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public override string Message { get; } = "could not find any matching user";

        public UserNotFoundException() : base()
        {
        }

        public UserNotFoundException(string message) : base(message)
        {
            Message = message;
        }
    }
}
