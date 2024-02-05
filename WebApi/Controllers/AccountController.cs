using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.IdentityServices.Interfaces;
using Services.Model;
using Services.Model.Account;
using WebApi.Infractructure.Response;

namespace WebApi.Controllers
{
    /// <summary>
    /// Login and registration account
    /// </summary>
    /// <seealso cref="BaseApiController" />
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class AccountController : BaseApiController
	{
		private readonly IAppUserManager _userManager;

	    /// <summary>
	    /// Initializes a new instance of the <see cref="AccountController" /> class.
	    /// </summary>
	    /// <param name="userManager">The user manager.</param>
	    public AccountController(IAppUserManager userManager)
		{
		    _userManager = userManager;
		}

        /// <summary>
        /// Registrations the specified user and login.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Bearer token</returns>
        /// <response code="200">Success</response>
        /// <response code="409">User with specific email already exist</response>
        /// <response code="409">Identity errors with model</response>
        /// <response code="409">Model validation errors</response>
        [HttpPost]
        [AllowAnonymous]
        [Route("registration")]
        [ProducesResponseType(typeof (ResponseWrapper<LoginResponseModel>), 200)]
        public async Task<LoginResponseModel> Registration([FromBody]RegistrationRequestModel model)
        {
            return await _userManager.RegisterUserAsync(model);
        }

        /// <summary>
        /// Registrations the specified external user.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Bearer token</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("login/external")]
        [ProducesResponseType(typeof(ResponseWrapper<LoginResponseModel>), 200)]
        public async Task<LoginResponseModel> ExternalLogin([FromBody] ExternalLoginRequestModel model)
        {
            return await _userManager.ExternalLoginAsync(model);
        }

	    /// <summary>
	    /// Registrations the specified external user.
	    /// </summary>
	    /// <param name="model">The model.</param>
	    /// <returns>Bearer token</returns>
	    [HttpPost]
	    [AllowAnonymous]
	    [Route("login/instagram")]
	    [ProducesResponseType(typeof(ResponseWrapper<LoginResponseModel>), 200)]
	    public async Task<LoginResponseModel> InstagramLogin([FromBody] InstagramLoginRequestModel model)
	    {
	        return await _userManager.LoginViaInstagramAsync(model);
	    }

        /// <summary>
        /// Logins the specified user.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Bearer token</returns>
        /// <response code="200">Success</response>
        /// <response code="404">Can't find user with specific email</response>
        /// <response code="409">Usert with specific email already exist</response>
        /// <response code="409">Incorrect Password</response>
        /// <response code="409">Identity errors with model</response>
        /// <response code="409">Model validation errors</response>
        [HttpPost]
		[AllowAnonymous]
		[Route("login")]
		[ProducesResponseType(typeof(ResponseWrapper<LoginResponseModel>), 200)]
		public async Task<LoginResponseModel> Login([FromBody] LoginRequestModel model)
		{
            return await _userManager.LoginAsync(model);
		}

        /// <summary>
        /// Logouts this instance.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("logout")]
        [ProducesResponseType(typeof(ResponseWrapper<SuccessModel>), 200)]
        public async Task<SuccessModel> Logout()
        {
            var authHeader = Request.Headers["Authorization"];
            await _userManager.DeauthorizeUser(UserId, authHeader);
            return new SuccessModel();
        }

        /// <summary>
		/// Restore the password.
		/// </summary>
		/// <returns></returns>
		/// <response code="200">Success</response>
		/// <response code="404">Can't find user with specific email</response>
		/// <response code="409">Identity errors with model</response>
		/// <response code="409">Model validation errors</response>
		[AllowAnonymous]
		[HttpPost]
	    [Route("forgot")]
		[ProducesResponseType(typeof(ResponseWrapper<SuccessModel>), 200)]
		public async Task<SuccessModel> ForgotPassword([FromBody]ResetPasswordModel model)
		{
			await _userManager.ResetUserPasswordAsync(model.Email);
            return new SuccessModel();
		}

        /// <summary>
		/// Restore the password from link
		/// </summary>
		/// <returns></returns>
		/// <response code="200">Success</response>
		/// <response code="404">Can't find user with specific email</response>
		/// <response code="409">Identity errors with model</response>
		/// <response code="409">Model validation errors</response>
		[AllowAnonymous]
        [HttpPost]
        [Route("forgot-send-link")]
        [ProducesResponseType(typeof(ResponseWrapper<SuccessModel>), 200)]
        public async Task<SuccessModel> GetLinkForgotPassword([FromBody]ResetPasswordModel model)
        {
            await _userManager.ForgotPasswordSendLinkAsync(model.Email);
            return new SuccessModel();
        }

        /// <summary>
        /// Changes the password from forgot passwordlink.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <response code="200">Success</response>
        /// <response code="409">Identity errors with model</response>
        /// <response code="409">Model validation errors</response>
        [AllowAnonymous]
        [HttpPost]
        [Route("forgot-change-password")]
        [ProducesResponseType(typeof(ResponseWrapper<LoginResponseModel>), 200)]
        public async Task<LoginResponseModel> ChangePassword([FromBody] ForgotPasswordRequestModel model)
        {
            return await _userManager.ChangeUserForgotPasswordAsync(model);
        }

        /// <summary>
        /// Changes the password.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <response code="200">Success</response>
        /// <response code="409">Identity errors with model</response>
        /// <response code="409">Model validation errors</response>
        [HttpPost]
	    [Route("change-password")]
	    [ProducesResponseType(typeof(ResponseWrapper<LoginResponseModel>), 200)]
	    public async Task<LoginResponseModel> ChangePassword([FromBody] ChangePasswordRequestModel model)
        {
            return await _userManager.ChangeUserPasswordAsync(model, UserId);
        }

	    /// <summary>
        /// Update user device info
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("device-info")]
        [ProducesResponseType(typeof(ResponseWrapper<SuccessModel>), 200)]
        public async Task<SuccessModel> UpdateDeviceInfo([FromBody]DeviceInfoModel model)
        {
            await _userManager.UpdateUserDeviceInfoAsync(UserId, model);
            return new SuccessModel();
        }

        /// <summary>
        /// Update push notification counter
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("unread-messages/count")]
        [ProducesResponseType(typeof(ResponseWrapper<SuccessModel>), 200)]
        public async Task<SuccessModel> SetUnreadMessagesCount([FromBody]UnreadMessageCounterModel model)
        {
            await _userManager.SetUnreadMessagesCountAsync(UserId, model.Counter);
            return new SuccessModel();
        }

        /// <summary>
        /// Verification code
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Validation result</returns>
        /// <response code="200">Success</response>
        /// <response code="409">Identity errors with model</response>
        /// <response code="409">Model validation errors</response>
        [HttpPost]
        [AllowAnonymous]
        [Route("check-code")]
        [ProducesResponseType(typeof(ResponseWrapper<BaseResponseModel>), 200)]
        public async Task<BaseResponseModel> VerifCodeOrganizer([FromBody] CheckVerificationRequest model)
        {
            return await _userManager.CheckVerificationCodeAsync(model);
        }

        /// <summary>
        /// Verifide entity code and send next code
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>validation result, message</returns>
        /// <response code="200">Success</response>
        /// <response code="409">Model validation errors</response>
        [HttpPost]
        [AllowAnonymous]
        [Route("send-code")]
        [ProducesResponseType(typeof(ResponseWrapper<BaseResponseModel>), 200)]
        public async Task<BaseResponseModel> SendVerificationCodeAsync([FromBody] SendVerificationCodeModelRequest model)
        {
            return await _userManager.SendVerificationCodeAsync(model);
        }
    }
}
