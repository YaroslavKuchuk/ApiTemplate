﻿namespace Services.Model.Messages.Enums
{
	public enum MessageState : short
	{
		NotSended = 0,
		Sended = 1,
		Pending = 2,
		InProgress = 3,
		Queued = 4
	}
}