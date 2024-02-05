using System.Linq;
using System.Threading.Tasks;
using Core.Data;
using Core.Data.Repositories;
using Core.Entities;
using Services.Interfaces;

namespace Services.Implementation
{
    public class UserTokenService : IUserTokenService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<UserToken> _userTokenRepository;
        private readonly IRepository<UserDevice> _userDeviceRepository;

        public UserTokenService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _userTokenRepository = _unitOfWork.Repository<UserToken>();
            _userDeviceRepository = unitOfWork.Repository<UserDevice>();
        }

        public async Task RemoveTokenIfExists(long deviceId)
        {
            var devices = await _userTokenRepository.FindByAsync(x => x.UserDeviceId == deviceId);
            var device = devices.FirstOrDefault();

            if (device != null)
            {
                _userTokenRepository.Delete(device);
            }
        }

        public async Task<UserToken> AddNewToken(long userId, long deviceId, string token)
        {
            await RemoveTokenIfExists(deviceId);
            await _unitOfWork.SaveChangesAsync();

            var userToken = new UserToken { UserId = userId, UserDeviceId = deviceId, AuthToken = token };

            await _userTokenRepository.Insert(userToken);
            await _unitOfWork.SaveChangesAsync();

            return userToken;
        }

        public async Task<UserDevice> GetDeviceByToken(string token)
        {
            var userToken = await _userTokenRepository.FindByAsync(x => x.AuthToken.Equals(token));
            var authToken = userToken.Select(x => x.UserDevice).FirstOrDefault();

            return authToken;
        }

        public async Task<UserToken> GetUserToken(long userId)
        {
            var userTokens = await _userTokenRepository.GetFilteredIncludingAsync(x => x.UserId == userId, d=>d.UserDevice);
            return userTokens.LastOrDefault();
        }

        public async Task RemoveAllUserTokens(long userId)
        {
            var userTokenList = await _userTokenRepository.FindByAsync(x => x.UserId == userId);

            foreach (var token in userTokenList)
            {
                _userTokenRepository.Delete(token);
            }

           await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveUserToken(string token)
        {
            var authTokens = await _userTokenRepository.FindByAsync(t => t.AuthToken == token);
            if (authTokens.Count == 0) return;

            var authToken = authTokens.FirstOrDefault();
            var dbName = _unitOfWork.GetDbName();
            var deleteCommand = $"delete from {dbName}.{_unitOfWork.GetTableName<UserToken>()} where Id={authToken.Id}; " +
                                $"delete from {dbName}.{_unitOfWork.GetTableName<UserDevice>()} where Id={authToken.UserDeviceId}";
            await _unitOfWork.ExecuteSqlCommand(deleteCommand);
        }
    }
}
