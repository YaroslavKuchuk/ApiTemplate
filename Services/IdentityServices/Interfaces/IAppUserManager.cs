using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.Entities;
using Services.IdentityServices.Enum;
using Services.Model;
using Services.Model.Account;
using Services.Model.AdminSection.Admin;
using Services.Model.AdminSection.User;
using Services.Model.User;

namespace Services.IdentityServices.Interfaces
{
	public interface IAppUserManager
	{
		#region Admin section

		Task<List<TEntity>> GetPreviewListAsync<TEntity>(int count, int offset, Expression<Func<User, bool>> predicate, params Expression<Func<User, object>>[] includeProperties) where TEntity : class;
		Task SetScreenStatusAsync(long userId, bool status);
	    Task CreateAdminAsync(NewAdminModel model);
        Task<SendPasswordCallback> AddAdminAsync(AdminCreateModel model);
	    Task DeleteUser(long userId);

        #endregion

        Task SetUnreadMessagesCountAsync(long userId, int counter);
	    Task UpdateUserDeviceInfoAsync(long userId, DeviceInfoModel model);

	    Task DeauthorizeUser(long userId, string authHeader);
        Task<LoginResponseModel> RegisterUserAsync(RegistrationRequestModel model);
		Task<LoginResponseModel> LoginAsync(LoginRequestModel model);
        Task<LoginResponseModel> ExternalLoginAsync(ExternalLoginRequestModel model);
	    Task<LoginResponseModel> LoginViaInstagramAsync(InstagramLoginRequestModel model);
        Task<LoginResponseModel> ChangeUserPasswordAsync(ChangePasswordRequestModel model, long userId);

	    Task BlockUserAsync(long userId, bool status);
	    Task<bool> IsEmailUnique(string email);
	    Task<SendPasswordCallback> ChangeAdminPasswordAsync(ChangeAdminPasswordModel model, long userId);
	    Task CreateUserAsync(CreateUserAdminModel model);
	    Task<UserProfileModel> GetAnotherProfileInfoAsync(long userId, long currentUserId);
	    Task<UserProfileModel> GetProfileInfoAsync(long userId);

        Task<TEntity> GetProfileInfoAsync<TEntity>(long userId) where TEntity : class;
		Task<UserProfileModel> UpdateProfileInfoAsync<TEntity>(TEntity model, long userId) where TEntity : class;
		Task ResetUserPasswordAsync(string email);
        Task ForgotPasswordSendLinkAsync(string email);
        Task<LoginResponseModel> ChangeUserForgotPasswordAsync(ForgotPasswordRequestModel model);

        Task<bool> IsPhoneUnique(string phoneNumber);
        Task<BaseResponseModel> CheckVerificationCodeAsync(CheckVerificationRequest model);
        Task<BaseResponseModel> SendVerificationCodeAsync(SendVerificationCodeModelRequest model);
        Task<bool> CheckPermissionForUser(long userId, Permission permission);

    }
}