using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
