using System;

namespace Services.Model.Messages
{
	[Serializable]
	public class EmailMessage : NotificationMessage
	{
		public string To { get; set; }
		public string Subject { get; set; }
	}
}
