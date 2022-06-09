#nullable disable

using System;

namespace OkayegTeaTime.Database.EntityFrameworkModels
{
    public class ExceptionLog
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Origin { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }

        public ExceptionLog(int id, string type, string origin, string message, string stackTrace)
        {
            Id = id;
            Type = type;
            Origin = origin;
            Message = message;
            StackTrace = stackTrace;
        }

        public ExceptionLog(Exception ex)
        {
            Type = ex.GetType().FullName;
            Origin = ex.TargetSite?.Name;
            Message = ex.Message;
            StackTrace = ex.StackTrace;
        }
    }
}
