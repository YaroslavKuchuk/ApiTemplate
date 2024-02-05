using System;
using System.Threading.Tasks;
using Core.Data;
using Core.Data.Repositories;
using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Services.Interfaces;
using Services.Model.Account;
using Services.Model.Messages;
using Services.Model.Messages.Enums;

namespace Services.Implementation.Message
{
	public partial class QueueMessageService : IQueueMessageService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IRepository<QueueMessage> _queueMessageRepository;

		public QueueMessageService(UserManager<User> userManager, IUnitOfWork unitOfWork)
		{
			_userManager = userManager;
			_unitOfWork = unitOfWork;
			_queueMessageRepository = _unitOfWork.Repository<QueueMessage>();
		}

	    public async Task RemovePendingMessages(long userId)
	    {
	        var messages = await _queueMessageRepository.FindByAsync(x => x.UserId == userId);
	        foreach (var queueMessage in messages)
	        {
	            _queueMessageRepository.Delete(queueMessage);
	        }
	       await _unitOfWork.SaveChangesAsync();
	    }

		public async Task AddResetPasswordMessageAsync(SendPasswordCallback callback)
		{
			await _queueMessageRepository.Insert(new QueueMessage
			{
				UserId = callback.User.Id,
				AttemptsCount = 0,
				CreateDate =  DateTime.UtcNow,
				UpdateDate =  DateTime.UtcNow,
				MessageState = (short)MessageState.NotSended,
				SerializedMessage = CreatePasswordMessage(callback),
				MessageType = typeof(EmailMessage).Name
			});

			await _unitOfWork.SaveChangesAsync();
		}

		public async Task AddChangePasswordMessageAsync(SendPasswordCallback callback)
		{
		    await _queueMessageRepository.Insert(new QueueMessage
			{
				UserId = callback.User.Id,
				AttemptsCount = 0,
				CreateDate =  DateTime.UtcNow,
				UpdateDate =  DateTime.UtcNow,
				MessageState = (short)MessageState.NotSended,
				SerializedMessage = CreatePasswordMessage(callback),
				MessageType = typeof(EmailMessage).Name
			});

			await _unitOfWork.SaveChangesAsync();
		}
	}
}
