using System;

namespace Services.Model.Messages
{
	[Serializable]
	public abstract class NotificationMessage
	{
		public string Body { get; set; }
	}
}
