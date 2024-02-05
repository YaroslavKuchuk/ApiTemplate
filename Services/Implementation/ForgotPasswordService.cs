using Common.Exceptions;
using Core.Data;
using Core.Data.Repositories;
using Core.Entities;
using Core.Enums;
using Resources;
using Services.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Implementation
{
    public class ForgotPasswordService : IForgotPasswordService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<UserForgotPassword> _forgotPasswordRepository;
        

        public ForgotPasswordService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _forgotPasswordRepository = _unitOfWork.Repository<UserForgotPassword>();
        }

        public async Task<UserForgotPassword> SetForgotPasswordDataAsync(long userId, int deltaExpDate)
        {
            var dateTimeUTC = DateTime.UtcNow;
            var oldRequests = await _forgotPasswordRepository.FindByAsync(x => x.UserId == userId && x.Status == UserForgotPasswordStatus.Active);
            foreach (var item in oldRequests)
            {
                item.Status = UserForgotPasswordStatus.NotActive;
                _forgotPasswordRepository.Update(item);
            }
            var userForgotPassword = new UserForgotPassword()
            {
                CreateDate = dateTimeUTC,
                Guid = Guid.NewGuid().ToString(),
                Status = UserForgotPasswordStatus.Active,
                ExpireDate = dateTimeUTC.AddDays(deltaExpDate),
                UserId = userId
            };

            await _forgotPasswordRepository.Insert(userForgotPassword);
            await _unitOfWork.SaveChangesAsync();

            return userForgotPassword;
        }

        public async Task<UserForgotPassword> ValidateForgotPasswordrequest(string guid)
        {
            var passwordRequest = (await _forgotPasswordRepository.FindByAsync(fp => fp.Guid == guid)).FirstOrDefault();
            if (passwordRequest == null)
            {
                throw new ApiException(Account.ResetPasswordNotRequested);
            }

            if (passwordRequest.ExpireDate < DateTime.UtcNow || passwordRequest.Status != UserForgotPasswordStatus.Active)
            {
                throw new ApiException(Account.ResetPasswordRequestExpired);
            }

            return passwordRequest;
        }

        public async Task UpdatePasswordAsync(UserForgotPassword passwordRequest)
        {
            passwordRequest.Status = UserForgotPasswordStatus.NotActive;
            _forgotPasswordRepository.Update(passwordRequest);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
