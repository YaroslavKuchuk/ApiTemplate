using Common.Exceptions.Account;
using Common.Exceptions.User;
using Core.Entities;
using Services.IdentityServices.Enum;
using Services.Model.Account;
using Services.Model.AdminSection.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Common.Settings;
using Common.Settings.NotificationSettings;
using Core.Enums;
using Core.Enums.Notification;
using Core.IdentityEntities;
using Services.IdentityServices.IdentityHelpers;
using Services.Interfaces;
using Services.Model.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Resources;
using Serilog;
using Services.Helpers;
using Services.IdentityServices.Interfaces;
using Services.Interfaces.Notifications;
using Services.Model.AdminSection.User;
using Services.Model.Content;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using UserLoginInfo = Microsoft.AspNetCore.Identity.UserLoginInfo;
using Services.Model;

namespace Services.IdentityServices
{
    public class AppUserManager : IAppUserManager
    {
        private readonly string _tokenKey;
        private readonly string _issuer;
        private readonly FacebookSettings _fbSettings;
        private readonly InstagramSettings _instagramSettings;
        private readonly ForgotPasswordSettings _forgotPasswordSettings;

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<AppRole> _roleManager;

        private readonly IUserDeviceService _userDeviceService;
        private readonly IUserTokenService _userTokenService;
        private readonly IUserService _userService;
        private readonly IPermissionService _permissionService;
        private readonly IMediaService _mediaService;
        private readonly IEmailNotificationService _emailNotificationService;
        private readonly IPushNotificationService _pushNotificationService;
        private readonly IActivationCodeService _activationCodeService;
        private readonly IForgotPasswordService _forgotPasswordService;

        private readonly ILogger Log = Serilog.Log.ForContext<AppUserManager>();

        public AppUserManager(IUserDeviceService userDeviceService, IUserTokenService userTokenService, IActivationCodeService activationCodeService,
            IUserService userService, ISettingService settingService, IMediaService mediaService, IEmailNotificationService emailNotificationService,
            IPushNotificationService pushNotificationService, IConfiguration config, UserManager<User> userManager, SignInManager<User> signInManager, 
            RoleManager<AppRole> roleManager, IForgotPasswordService forgotPasswordService, IPermissionService permissionService)
        {
            _userDeviceService = userDeviceService;
            _userTokenService = userTokenService;
            _userService = userService;
            _mediaService = mediaService;
            _emailNotificationService = emailNotificationService;
            _pushNotificationService = pushNotificationService;
            _activationCodeService = activationCodeService;
            _forgotPasswordService = forgotPasswordService;
            _permissionService = permissionService;

            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _tokenKey = config["Tokens:Key"];
            _issuer = config["Tokens:Issuer"];
            _fbSettings = settingService.GetSettings<FacebookSettings>().Result;
            _instagramSettings = settingService.GetSettings<InstagramSettings>().Result;
            _forgotPasswordSettings = settingService.GetSettings<ForgotPasswordSettings>().Result;
        }

        public async Task<SendPasswordCallback> AddAdminAsync(AdminCreateModel model)
        {
            if (await _userService.FindByEmailAsync(model.Email) != null)
            {
                throw new EmailExistException(Account.UserExist);
            }

            var user = Mapper.Map<User>(model);
            var passwordOptions = _signInManager.Options.Password;
            var password = Generators.GenerateRandomPassword(passwordOptions);

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                var errors = (List<IdentityError>)result.Errors;
                throw new IdentityException(string.Join("; ",
                    Array.ConvertAll(errors.ToArray(), i => i.Description.ToString())));
            }

            await _userManager.AddToRoleAsync(user, Role.Admin.ToString());

            return new SendPasswordCallback
            {
                Password = password,
                User = user,
                Subject = EmailTemplates.NewAdminRegisterSubject,
                Body = string.Format(EmailTemplates.NewAdminRegisterBody, $"{user.FirstName} {user.LastName}", password)
            };
        }

        public async Task CreateAdminAsync(NewAdminModel model)
        {
            try
            {
                if (_userManager.Users.Count(x => x.Email == model.Email) > 0)
                {
                    return;
                }

                var user = Mapper.Map<User>(model);
                var result = await _userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                {
                    var errors = (List<IdentityError>)result.Errors;
                    Log.Error(string.Join("; ", Array.ConvertAll(errors.ToArray(), i => i.Description.ToString())));
                }
                await _userManager.AddToRoleAsync(user, Role.Admin.ToString());
            }
            catch (Exception e)
            {
                Log.Error(e, "An {@Exception} has been occurred in CreateAdminAsync");
            }

        }

        public async Task DeleteUser(long userId)
        {
            try
            {
                var isNeedToCleanStorage = await _userService.DeleteUser(userId);
                if (isNeedToCleanStorage)
                {
                    await _mediaService.DeleteBucket(userId.ToString()); //TODO: check delete bucket
                }
                await _signInManager.SignOutAsync();
            }
            catch (Exception ex)
            {
                //Log.Error(ex, "An {@Exception} has been occurred when deleting user", ex);
                throw ex;
            }
        }

        public async Task ResetUserPasswordAsync(string email)
        {
            var user = await _userService.FindByEmailAsync(email);

            if (user == null || user.IsDelete)
            {
                throw new UserNotFoundException(Account.UserNotFound);
            }

            if (user.PasswordHash == null)
            {
                throw new ChangePasswordException(Account.NotForExternal);
            }

            user.UpdateDate = DateTime.UtcNow;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var passwordOptions = _signInManager.Options.Password;
            var password = Generators.GenerateRandomPassword(passwordOptions);

            var result = await _userManager.ResetPasswordAsync(user, token, password);

            if (!result.Succeeded)
            {
                var errors = (List<IdentityError>)result.Errors;
                throw new IdentityException(string.Join("; ", Array.ConvertAll(errors.ToArray(), i => i.Description.ToString())));
            }

            var callback = new SendPasswordCallback { Password = password, User = user, Subject = EmailTemplates.ForgotPasswordSubject, Body = string.Format(EmailTemplates.ForgotPasswordBody, $"{user.FirstName} {user.LastName}", password) };
            try
            {
                await _emailNotificationService.SendMessageAsync(callback.Body, callback.Subject, callback.User.Id, ActivityType.ForgotPassword);
            }
            catch (Exception) { }
        }

        public async Task ForgotPasswordSendLinkAsync(string email)
        {
            var user = await _userService.FindByEmailAsync(email);

            if (user == null)
            {
                throw new UserNotFoundException(Account.UserNotFound);
            }

            if (user.PasswordHash == null)
            {
                throw new ChangePasswordException(Account.NotForExternal);
            }

            user.UpdateDate = DateTime.UtcNow;
            var userForgotPassword = await _forgotPasswordService.SetForgotPasswordDataAsync(user.Id, _forgotPasswordSettings.ForgotPasswordDeltaExpDay);

            string body = string.Format(EmailTemplates.ForgotPasswordLinkBody, _forgotPasswordSettings.ForgotPasswordWebPortalLink, userForgotPassword.Guid);
            await _emailNotificationService.SendMessageAsync(body, EmailTemplates.ForgotPasswordSubject, user.Id, ActivityType.ForgotPassword);
        }

        public async Task<LoginResponseModel> ChangeUserForgotPasswordAsync(ForgotPasswordRequestModel model)
        {
            var userForgotPassword = await _forgotPasswordService.ValidateForgotPasswordrequest(model.Guid);
            var user = await _userManager.FindByIdAsync(userForgotPassword.UserId.ToString());

            if (user == null)
            {
                throw new UserNotFoundException(Account.UserNotFound);
            }
            var newPasswordHash = _userManager.PasswordHasher.HashPassword(user, model.Password);
            user.PasswordHash = newPasswordHash;
            user.UpdateDate = DateTime.Now;
            await _userManager.UpdateAsync(user);
            await _forgotPasswordService.UpdatePasswordAsync(userForgotPassword);
            await _signInManager.SignInAsync(user, isPersistent: false);

            user.UpdateDate = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            var lastToken = await _userTokenService.GetUserToken(user.Id);
            return await ObtainAccessAndSetDevice(new LoginRequestModel
            {
                Email = user.Email,
                DeviceId = lastToken.UserDeviceId?.ToString() ?? string.Empty,
                Password = model.Password,
                DeviceToken = lastToken.UserDevice.DevicePushToken,
                OsType = (OsType)lastToken.UserDevice.OsType
            }, user);
        }

        public async Task<LoginResponseModel> RegisterUserAsync(RegistrationRequestModel model)
        {
            if (await _userService.FindByEmailAsync(model.Email) != null)
            {
                throw new EmailExistException(Account.UserExist);
            }

            if (!await _activationCodeService.IsActiveCode(model.Email))
            {
                throw new EmailExistException(Account.EmailIsNotValidated);
            }

            var user = Mapper.Map<User>(model);
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errors = (List<IdentityError>)result.Errors;
                throw new IdentityException(string.Join("; ", Array.ConvertAll(errors.ToArray(), i => i.Description.ToString())));
            }

            await _userManager.AddToRoleAsync(user, Role.User.ToString());
            if (!string.IsNullOrWhiteSpace(model.AvatarFile))
            {
                model.AvatarFileType = !string.IsNullOrWhiteSpace(model.AvatarFileType) ? model.AvatarFileType : "jpg";
                var avatarUrl = await _mediaService.UploadFileToBucketFromBase64(
                    new Base64FileModel
                    {
                        Base64 = model.AvatarFile,
                        ContentType = $@"image/{model.AvatarFileType}",
                        FileName = $"{Guid.NewGuid()}.{model.AvatarFileType}"
                    }, user.Id, null, false);
                user.Image = avatarUrl;
                await _userManager.UpdateAsync(user);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            return await ObtainAccessAndSetDevice(model, user);
        }

        public async Task<LoginResponseModel> LoginAsync(LoginRequestModel model)
        {
            var user = await _userService.FindByEmailAsync(model.Email);

            if (user == null || user.IsDelete)
            {
                throw new UserNotFoundException(Account.UserNotFound);
            }
            Log.Information($"User {user.Email} has successfully logged in!");

            if (!await _userManager.CheckPasswordAsync(user, model.Password))
            {
                throw new IncorrectPasswordException(Account.PasswordIncorrect);
            }

            if (!user.IsActive)
            {
                throw new UserBlockedException(Account.UserBlocked);
            }
            await _signInManager.SignInAsync(user, isPersistent: false);

            user.EnablePush = user.EnableNotifications;
            user.UpdateDate = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            return await ObtainAccessAndSetDevice(model, user);
        }

        public async Task<LoginResponseModel> ExternalLoginAsync(ExternalLoginRequestModel model)
        {
            FacebookModel facebookModel = await FacebookHelper.GetFbUserAsync(model.ExternalAccessToken, _fbSettings);

            UserLoginInfo userLoginInfo = new UserLoginInfo("FBProviderName", facebookModel.Id, facebookModel.UserName);
            User existingExternalUser = await _userManager.FindByLoginAsync("FBProviderName", facebookModel.Id);

            if (existingExternalUser == null)
            {
                User existingUser = await _userService.FindByEmailAsync(facebookModel.Email);
                if (existingUser != null)
                {
                    if (!existingUser.IsActive)
                    {
                        throw new UserBlockedException(Account.UserBlocked);
                    }

                    return await ObtainAccessAndSetDevice(model, existingUser);
                }

                User user = Mapper.Map<User>(facebookModel);

                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    var errors = (List<IdentityError>)result.Errors;
                    throw new IdentityException(string.Join("; ", Array.ConvertAll(errors.ToArray(), i => i.Description.ToString())));
                }

                await _userManager.AddLoginAsync(user, userLoginInfo);
                await _userManager.AddToRoleAsync(user, Role.User.ToString());

                return await ObtainAccessAndSetDevice(model, user);
            }

            if (existingExternalUser.IsDelete)
            {
                throw new UserNotFoundException(Account.UserNotFound);
            }

            if (!existingExternalUser.IsActive)
            {
                throw new UserBlockedException(Account.UserBlocked);
            }

            return await ObtainAccessAndSetDevice(model, existingExternalUser);
        }

        public async Task<LoginResponseModel> LoginViaInstagramAsync(InstagramLoginRequestModel model)
        {
            var instagramUser = await InstagramHelper.GetInstagramUserAsync(model.AuthorizationCode, _instagramSettings);
            if (!instagramUser.Id.HasValue)
            {
                throw new UserNotFoundException(Account.UserFetchApi);
            }

            UserLoginInfo userLoginInfo = new UserLoginInfo("InstagramProviderName", instagramUser.Id.ToString(), instagramUser.Username);
            User existingExternalUser = await _userManager.FindByLoginAsync("InstagramProviderName", instagramUser.Id.ToString());

            if (existingExternalUser == null)
            {
                User existingUser = await _userService.FindByEmailAsync(instagramUser.Username);
                if (existingUser != null)
                {
                    if (!existingUser.IsActive)
                    {
                        throw new UserBlockedException(Account.UserBlocked);
                    }

                    return await ObtainAccessAndSetDevice(model, existingUser);
                }

                var user = Mapper.Map<User>(instagramUser);

                _userManager.Options.User.RequireUniqueEmail = false; // disable email validation for this request

                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    var errors = (List<IdentityError>)result.Errors;
                    throw new IdentityException(string.Join("; ",
                        Array.ConvertAll(errors.ToArray(), i => i.Description.ToString())));
                }

                await _userManager.AddLoginAsync(user, userLoginInfo);
                await _userManager.AddToRoleAsync(user, Role.User.ToString());

                return await ObtainAccessAndSetDevice(model, user);
            }
            if (existingExternalUser.IsDelete)
            {
                throw new UserNotFoundException(Account.UserNotFound);
            }
            if (!existingExternalUser.IsActive)
            {
                throw new UserBlockedException(Account.UserBlocked);
            }

            return await ObtainAccessAndSetDevice(model, existingExternalUser);
        }

        public async Task<LoginResponseModel> ChangeUserPasswordAsync(ChangePasswordRequestModel model, long userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null || user.IsDelete)
            {
                throw new UserNotFoundException(Account.UserNotFound);
            }
            if (user.PasswordHash == null)
            {
                throw new ChangePasswordException(Account.NotForExternal);
            }

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.Password);

            if (!result.Succeeded)
            {
                var errors = (List<IdentityError>)result.Errors;
                throw new IdentityException(string.Join("; ",
                    Array.ConvertAll(errors.ToArray(), i => i.Description.ToString())));
            }

            var callback = new SendPasswordCallback
            {
                Password = model.Password,
                User = user,
                Subject = EmailTemplates.ChangePasswordSubject,
                Body = string.Format(EmailTemplates.ChangePasswordBody, $"{user.FirstName} {user.LastName}",
                    model.Password)
            };
            try
            {
                await _emailNotificationService.SendMessageAsync(callback.Body, callback.Subject, callback.User.Id,
                    ActivityType.ChangePassword);
            }
            catch (Exception)
            {
            }

            await _signInManager.SignInAsync(user, isPersistent: false);

            user.UpdateDate = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            var lastToken = await _userTokenService.GetUserToken(userId);
            return await ObtainAccessAndSetDevice(new LoginRequestModel
            {
                Email = callback.User.Email,
                DeviceId = lastToken.UserDeviceId?.ToString() ?? string.Empty,
                Password = model.Password,
                DeviceToken = lastToken.UserDevice.DevicePushToken,
                OsType = (OsType)lastToken.UserDevice.OsType
            }, user);
        }

        public async Task BlockUserAsync(long userId, bool status)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null || user.IsDelete)
            {
                throw new UserNotFoundException(Account.UserSpecificNotFound);
            }
            user.IsActive = status;
            await _userManager.UpdateAsync(user);
        }

        public async Task<bool> IsEmailUnique(string email)
        {
            var iEmailCorrect = Regex.IsMatch(email,
                @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
                RegexOptions.IgnoreCase);
            if (!iEmailCorrect)
            {
                throw new EmailIsNotValidException(Account.EmailIsNotValid);
            }
            return await _userService.IsEmailUnique(email);
        }

        public async Task<bool> IsPhoneUnique(string phoneNumber)
        {
            return await _userService.IsPhoneUnique(phoneNumber);
        }

        public async Task<SendPasswordCallback> ChangeAdminPasswordAsync(ChangeAdminPasswordModel model, long userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null || user.IsDelete)
            {
                throw new UserNotFoundException(Account.UserNotFound);
            }

            if (user.PasswordHash == null)
            {
                throw new ChangePasswordException(Account.NotForExternal);
            }

            await _userManager.RemovePasswordAsync(user);

            var result = await _userManager.AddPasswordAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errors = (List<IdentityError>)result.Errors;
                throw new IdentityException(string.Join("; ", Array.ConvertAll(errors.ToArray(), i => i.Description.ToString())));
            }

            return new SendPasswordCallback
            {
                Password = model.Password,
                User = user,
                Subject = EmailTemplates.ChangePasswordSubject,
                Body = EmailTemplates.ChangePasswordBody
            };
        }

        public async Task CreateUserAsync(CreateUserAdminModel model)
        {
            if (await _userService.FindByEmailAsync(model.Email) != null)
            {
                throw new EmailExistException(Account.UserExist);
            }

            if (!await _activationCodeService.IsActiveCode(model.Email))
            {
                throw new EmailExistException(Account.EmailIsNotValidated);
            }

            var user = Mapper.Map<User>(model);
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errors = (List<IdentityError>)result.Errors;
                throw new IdentityException(string.Join("; ", Array.ConvertAll(errors.ToArray(), i => i.Description.ToString())));
            }

            await _userManager.AddToRoleAsync(user, Role.User.ToString());
        }

        public async Task<UserProfileModel> GetAnotherProfileInfoAsync(long userId, long currentUserId)
        {
            var user = await _userService.FindByIdAsync(userId);
            if (user == null || user.IsDelete)
            {
                throw new UserNotFoundException(string.Format(Account.UserSpecificId, userId));
            }

            return Mapper.Map<UserProfileModel>(user);
        }

        public async Task<TEntity> GetProfileInfoAsync<TEntity>(long userId) where TEntity : class
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null || user.IsDelete)
            {
                throw new UserNotFoundException(string.Format(Account.UserSpecificId, userId));
            }

            return Mapper.Map<TEntity>(user);
        }

        public async Task<UserProfileModel> GetProfileInfoAsync(long userId)
        {
            var user = await _userService.FindByIdAsync(userId);
            if (user == null || user.IsDelete)
            {
                throw new UserNotFoundException(string.Format(Account.UserSpecificId, userId));
            }

            var profileModel = Mapper.Map<UserProfileModel>(user);
            return profileModel;
        }

        public async Task<UserProfileModel> UpdateProfileInfoAsync<TEntity>(TEntity model, long userId) where TEntity : class
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null || user.IsDelete)
            {
                throw new UserNotFoundException(Account.UserNotFound);
            }
            Mapper.Map(model, user);
            user.UpdateDate = DateTime.UtcNow;
            if (user.PasswordHash == null)
            {
                _userManager.Options.User.RequireUniqueEmail = false; // disable email validation for this request
            }

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = (List<IdentityError>)result.Errors;
                throw new UpdateProfileException(string.Join("; ", errors));
            }
            var dbUser = await _userService.FindByIdAsync(userId);
            return Mapper.Map<UserProfileModel>(dbUser);
        }

        public async Task<List<TEntity>> GetPreviewListAsync<TEntity>(int count, int offset, Expression<Func<User, bool>> predicate, params Expression<Func<User, object>>[] includeProperties) where TEntity : class
        {
            var users = await FilterAndPaginateAsync(count, offset, predicate, includeProperties);
            return Mapper.Map<List<TEntity>>(users);
        }

        public async Task UpdateUserDeviceInfoAsync(long userId, DeviceInfoModel model)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null || user.IsDelete)
            {
                throw new UserNotFoundException(string.Format(Account.UserSpecificId, userId));
            }

            var device = await _userDeviceService.FindBySingle(e => e.DeviceId.Equals(model.DeviceId));

            if (device != null)
            {
                await _userDeviceService.SetDeviceToken(device, model.DeviceToken);
            }

            await _userManager.UpdateAsync(user);
        }

        public async Task SetUnreadMessagesCountAsync(long userId, int counter)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null || user.IsDelete)
            {
                throw new UserNotFoundException(string.Format(Account.UserSpecificId, userId));
            }

            user.UnreadMessageCount = counter;

            await _userManager.UpdateAsync(user);
        }

        public async Task SetScreenStatusAsync(long userId, bool status)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null || user.IsDelete)
            {
                throw new UserNotFoundException(string.Format(Account.UserSpecificId, userId));
            }

            user.IsActive = status;
            user.UpdateDate = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);
        }

        public async Task DeauthorizeUser(long userId, string authHeader)
        {
            var bearerToken = BearerTokenManager.GetCurrentUserToken(authHeader);
            await _userTokenService.RemoveUserToken(bearerToken);
            await _signInManager.SignOutAsync();
            await _userService.DisableNotificationWhenLoggedOut(userId, new UserSettingsModel { EnablePush = false }); //disable push notification
        }

        public async Task<BaseResponseModel> CheckVerificationCodeAsync(CheckVerificationRequest model)
        {
            return await CheckCodeAsync(model.Entity, model.Code);
        }

        public async Task<BaseResponseModel> SendVerificationCodeAsync(SendVerificationCodeModelRequest model)
        {
            return model.IsEmail ? await SendCodeToEmail(model) : await SendCodeToPhone(model);
        }

        public async Task<bool> CheckPermissionForUser(long userId, Permission permission)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null || user.IsDelete)
            {
                throw new UserNotFoundException(string.Format(Account.UserSpecificId, userId));
            }
            var roles = (await _userManager.GetRolesAsync(user)).ToList();
            return await  _permissionService.CheckPermissionInRole(roles, permission);
        }

        #region private methods

        private async Task<List<User>> FilterAndPaginateAsync(int count, int offset, Expression<Func<User, bool>> predicate, params Expression<Func<User, object>>[] includeProperties)
        {
            var users = IncludeProperties(includeProperties);

            users = predicate != null ? users.Where(predicate) : users;

            users = users
                .OrderByDescending(x => x.UpdateDate)
                .Skip(offset)
                .Take(count);

            return await users.ToListAsync();
        }

        private IQueryable<User> IncludeProperties(params Expression<Func<User, object>>[] includeProperties)
        {
            return includeProperties.Aggregate(_userManager.Users, (current, includeProperty) => current.Include(includeProperty));
        }


        private async Task<LoginResponseModel> ObtainAccessAndSetDevice(BaseAuthModel model, User user)
        {
            var roleNames = await _userManager.GetRolesAsync(user);
            var token = await BearerTokenManager.GenerateToken(user, roleNames, _roleManager, _tokenKey, _issuer);

            var device = await _userDeviceService.FindBySingle(e => e.DeviceId.Equals(model.DeviceId));

            if (device == null)
            {
                device = await _userDeviceService.AddDevice(model.DeviceId, model.DeviceToken, model.OsType);
            }
            else
            {
                await _userDeviceService.SetDeviceToken(device, model.DeviceToken);
            }

            await _userTokenService.AddNewToken(user.Id, device.Id, token);

            return new LoginResponseModel
            {
                UserId = user.Id,
                Token = token,
                UserName = user.UserName,
                Role = string.Join(",", roleNames),
                Image = user.Image
            };
        }

        private async Task<BaseResponseModel> CheckCodeAsync(string entity, string code)
        {
            var result = new BaseResponseModel()
            {
                IsValidate = await _activationCodeService.IsValidateCodeAsync(code, entity, DateTime.UtcNow)
            };
            if (!result.IsValidate)
                result.Message = Account.UserEntityCodeIsNotValidate;

            return result;
        }

        private async Task<BaseResponseModel> SendCodeToEmail(SendVerificationCodeModelRequest model)
        {
            if (!await IsEmailUnique(model.Entity))
            {
                return new BaseResponseModel()
                {
                    IsValidate = false,
                    Message = Account.UserExist
                };
            }

            var code = await _activationCodeService.ReRenderCode(model.Entity);
            await _emailNotificationService.SendMessageAsync(EmailTemplates.VerificationCodeSubject,
                string.Format(EmailTemplates.VerificationCodeBody, string.Empty, code), model.Entity, ActivityType.VerificationCode);

            return new BaseResponseModel()
            {
                IsValidate = true
            };
        }

        private async Task<BaseResponseModel> SendCodeToPhone(SendVerificationCodeModelRequest model)
        {
            if (!await IsPhoneUnique(model.Entity))
            {
                return new BaseResponseModel()
                {
                    IsValidate = false,
                    Message = Account.UserExist
                };
            }

            var code = await _activationCodeService.ReRenderCode(model.Entity);
            await _pushNotificationService.SendMessageAsync(code, PushTemplates.VerificationCodeRequestTitle, model.Entity, ActivityType.VerificationCode);
            return new BaseResponseModel()
            {
                IsValidate = true
            };
        }        

        #endregion

    }
}