using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
	public class QueueMessage : BaseEntity
	{
		public string ExceptionMessage { get; set; }
	    [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; }
		public long AttemptsCount { get; set; }
		public short MessageState { get; set; }
	    [Column(TypeName = "datetime")]
        public DateTime UpdateDate { get; set; }
		public byte[] SerializedMessage { get; set; }
        
		public virtual User User { get; set; }
        public long UserId { get; set; }

        public string MessageType { get; set; }
    }
}
