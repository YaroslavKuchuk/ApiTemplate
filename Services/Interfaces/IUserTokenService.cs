using System.Threading.Tasks;
using Core.Entities;

namespace Services.Interfaces
{
    public interface IUserTokenService
    {
        Task RemoveTokenIfExists(long deviceId);
        Task RemoveUserToken(string token);
        Task RemoveAllUserTokens(long userId);

        Task<UserToken> AddNewToken(long userId, long deviceId, string token);
        Task<UserDevice> GetDeviceByToken(string token);
        Task<UserToken> GetUserToken(long userId);
    }
}