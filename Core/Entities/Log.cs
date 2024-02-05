using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class Log
    {
        public long Id { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? EventDateTime { get; set; }

        public string Type { get; set; }

        public long? UserId { get; set; }

        public string Path { get; set; }

        public string RequestType { get; set; }

        public string InputParameters { get; set; }

        public string Response { get; set; }

        public string ErrorSource { get; set; }

        public string ErrorMessage { get; set; }

        public string InnerErrorMessage { get; set; }

        public string StackTrace { get; set; }

        public double ExecutionTime { get; set; }
    }
}
