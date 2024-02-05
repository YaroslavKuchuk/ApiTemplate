using System;
using System.Threading.Tasks;
using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Services.Model.Messages;
using Services.Model.Messages.Enums;

namespace Services.Implementation.Message
{
    public partial class QueueMessageService
	{
	    private readonly UserManager<User> _userManager;

        public async Task AddPushMessageAsync(User fromUser, User toUser, string body)
		{
		    await AddNewQueueMessageToRepository(fromUser, toUser, body);
		}
		
		private async Task AddNewQueueMessageToRepository(User fromUser, User toUser, string body)
		{
		    if (toUser.EnablePush)
		    {
                await IncraseUnreadMessageCount(toUser.Id);

                var pushMessage = new PushNotificationMessage
                {
                    Body = body,
                    From = fromUser.Email,
                    MessageType = PushMessageType.Message
                };

		        await _queueMessageRepository.Insert(new QueueMessage
                {
                    UserId = toUser.Id,
                    AttemptsCount = 0,
                    CreateDate =  DateTime.UtcNow,
                    UpdateDate =  DateTime.UtcNow,
                    MessageState = (short)MessageState.NotSended,
                    SerializedMessage = GetMessageBytes(pushMessage),
                    MessageType = typeof(PushNotificationMessage).Name
                });

                await _unitOfWork.SaveChangesAsync();
            }
		}

		private async Task IncraseUnreadMessageCount(long userId)
		{
			var user = await _userManager.FindByIdAsync(userId.ToString());
			user.UnreadMessageCount += 1;

			await _userManager.UpdateAsync(user);
		}
	}
}

