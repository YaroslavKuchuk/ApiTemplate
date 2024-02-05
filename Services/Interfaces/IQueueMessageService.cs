using System.Threading.Tasks;
using Core.Entities;
using Services.Model.Account;

namespace Services.Interfaces
{
    public interface IQueueMessageService
    {
        Task RemovePendingMessages(long userId);
        Task AddPushMessageAsync(User fromUser, User toUser, string body);

        Task AddResetPasswordMessageAsync(SendPasswordCallback callback);
        Task AddChangePasswordMessageAsync(SendPasswordCallback callback);
    }
}