using Core.Entities;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IForgotPasswordService
    {
        Task<UserForgotPassword> SetForgotPasswordDataAsync(long userId, int deltaExpDate);
        Task UpdatePasswordAsync(UserForgotPassword passwordRequest);
        Task<UserForgotPassword> ValidateForgotPasswordrequest(string guid);
    }
}
