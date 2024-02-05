using System;
using Common.Helpers;
using Services.Model.Messages.Enums;

namespace Services.Model.Messages
{
	public class SendResult
	{
		public bool Success => State == MessageState.Sended || State == MessageState.Queued;

		public string Error { get; private set; }

		public MessageState State { get; }

		public long MessageId { get; private set; }

		public SendResult(long id, MessageState state)
		{
			MessageId = id;
			State = state;
		}

		public SendResult(long id, Exception error)
		{
			MessageId = id;
			Error = LogUtility.BuildExceptionMessage(error);
			State = MessageState.Pending;
		}

		public SendResult(long id, string error)
		{
			MessageId = id;
			Error = error;
			State = MessageState.Pending;
		}
	}
}
