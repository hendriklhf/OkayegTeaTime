using System.Collections.Generic;

namespace OkayegTeaTimeCSharp.Messages.Interfaces
{
    public interface IChatMessage
    {
        public string DisplayName { get; }

        public List<string> LowerSplit { get; }

        public string Message { get; }

        public List<string> Split { get; }

        public string Username { get; }
    }
}
