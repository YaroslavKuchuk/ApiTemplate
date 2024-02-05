using System;
using Core.Enums;
using Services.Model.Messages.Enums;

namespace Services.Model.Messages
{
	[Serializable]
	public class PushNotificationMessage : NotificationMessage
	{
		private PushMessageType _messageType = PushMessageType.Message;

		public string DeviceToken { get; set; }

		public OsType? OsType { get; set; }

		public string From { get; set; }

		public int UnreadMessageCount { get; set; }

        public PushMessageType MessageType
		{
			get { return _messageType; }
			set { _messageType = value; }
		}
	}
}
