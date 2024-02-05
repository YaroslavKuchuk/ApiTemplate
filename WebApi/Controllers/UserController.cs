using System;
using System.Threading.Tasks;
using Common.Exceptions.Account;
using Core.Enums.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Helpers;
using Services.IdentityServices.Enum;
using Services.IdentityServices.Interfaces;
using Services.Interfaces;
using Services.Model;
using Services.Model.Pagination;
using Services.Model.User;
using WebApi.Infractructure.Response;

namespace WebApi.Controllers
{
    /// <summary>
    /// User manage for mobile app
    /// </summary>
    /// <seealso cref="Controller" />
    //[CustomAuthorize]
    [Route("api/[controller]")]
    public class UserController : BaseApiController
    {
		private readonly IAppUserManager _userManager;
        private readonly IUserService _userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController" /> class.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="userService">The user service</param>
        public UserController(IAppUserManager userManager, IUserService userService)
		{
			_userManager = userManager;
		    _userService = userService;
		}

        /// <summary>
        /// Gets the specified user info.
        /// </summary>
        /// <returns>User info</returns>
        /// <response code="200">Success</response>
        /// <response code="404">User not found</response>
        [HttpGet]
        [Route("{id:long}")]
        [ProducesResponseType(typeof(ResponseWrapper<UserProfileModel>), 200)]
        [Authorize(Policy = PermissionString.ViewUserInfo)]
        public async Task<UserProfileModel> GetUserInfo(long id)
        {
            return await _userManager.GetAnotherProfileInfoAsync(id, UserId);
        }

        /// <summary>
        /// Gets the own user info.
        /// </summary>
        /// <returns>User info</returns>
        /// <response code="200">Success</response>
        /// <response code="404">User not found</response>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(ResponseWrapper<UserProfileModel>), 200)]
        public async Task<UserProfileModel> Get()
        {
            return await _userManager.GetProfileInfoAsync(UserId);
        }

        /// <summary>
        /// Gets the specified user info by cipher user id.
        /// </summary>
        /// <returns>User info</returns>
        /// <response code="200">Success</response>
        /// <response code="404">User not found</response>
        [HttpGet]
        [Route("cipher")]
        [ProducesResponseType(typeof(ResponseWrapper<UserProfileModel>), 200)]
        public async Task<UserProfileModel> GetCipherUserInfo(string cipherUserId, int? page = PageNumber, int? pageSize = PageSize)
        {
            long id = 0;
            try
            {
                id = Convert.ToInt64(Crypto.ConvertHexToString(cipherUserId));
            }
            catch
            {
                throw new UserNotFoundException("Can't find user with specific id");
            }
            return await _userManager.GetAnotherProfileInfoAsync(id, UserId);
        }

        /// <summary>
        /// Gets users.
        /// </summary>
        /// <param name="searchQuery">The search criteria</param>
        /// <param name="pageNumber">The offset.</param>
        /// <param name="pageSize">The count.</param>
        /// <param name="orderBy">The order by</param>
        /// <returns>list of users</returns>
        /// <response code="200">Success</response>
        [HttpGet]
        [Route("list")]
        [ProducesResponseType(typeof(ResponseWrapper<PaginationResult<SearchUserModel>>), 200)]
        [Authorize(Policy = PermissionString.ViewAppUsers)]
        public async Task<PaginationResult<SearchUserModel>> GetAppUsers(string searchQuery = null, int? pageNumber = PageNumber, int? pageSize = PageSize, UserOrderBy orderBy = UserOrderBy.Name)
        {
            return await _userService.GetAppUsers(UserId, searchQuery, pageNumber.Value, pageSize.Value, orderBy);
        }

        /// <summary>
        /// Update the specified user info.
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="409">Identity errors with model</response>
        /// <response code="409">Model validation errors</response>
        [HttpPut]
        [Route("")]
        [ProducesResponseType(typeof(ResponseWrapper<UserProfileModel>), 200)]       
        public async Task<UserProfileModel> Put([FromBody]UpdateProfileRequestModel model)
        {
            return await _userManager.UpdateProfileInfoAsync(model, UserId);
        }

        /// <summary>
		/// Block certain user.
		/// </summary>
		/// <response code="200">Success</response>
		/// <response code="409">Identity errors with model</response>
		/// <response code="409">Model validation errors</response>
		[HttpPut]
        [Route("block/{id:long}")]
        [ProducesResponseType(typeof(ResponseWrapper<SuccessModel>), 200)]
        [Authorize(Policy = PermissionString.BlockUser)]
        public async Task<SuccessModel> BlockUser(long id)
        {
            await _userManager.BlockUserAsync(id, false);
            return new SuccessModel();
        }

        /// <summary>
        /// Unblock certain user.
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="409">Identity errors with model</response>
        /// <response code="409">Model validation errors</response>
        [HttpPut]
        [Route("unblock/{id:long}")]
        [ProducesResponseType(typeof(ResponseWrapper<SuccessModel>), 200)]
        [Authorize(Policy = PermissionString.UnBlockUser)]
        public async Task<SuccessModel> UnBlockUser(long id)
        {
            await _userManager.BlockUserAsync(id, true);
            return new SuccessModel();
        }

        /// <summary>
        /// Set update date for certain user.
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="409">Identity errors with model</response>
        /// <response code="409">Model validation errors</response>
        [HttpPut]
        [Route("set-update-date")]
        [ProducesResponseType(typeof(ResponseWrapper<SuccessModel>), 200)]
        public async Task<SuccessModel> ChangeUserUpdateDate()
        {
            await _userService.ChangeUserUpdateDate(UserId);
            return new SuccessModel();
        }

        /// <summary>
        /// Check if email unique
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("is-email-unique")]
        [ProducesResponseType(typeof(ResponseWrapper<bool>), 200)]
        public async Task<bool> IsEmailUnique([FromQuery]IsEmailUniqueModel model)
        {
            return await _userManager.IsEmailUnique(model.Email);
        }

        [HttpPut]
        [Route("update-settings")]
        [ProducesResponseType(typeof(ResponseWrapper<SuccessModel>), 200)]
        public async Task<SuccessModel> UpdateSettigs([FromBody]UserSettingsModel model)
        {
            await _userService.UpdateUserSettingsAsync(UserId, model);
            return new SuccessModel();
        }
	}
}
