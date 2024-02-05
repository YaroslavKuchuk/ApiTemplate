using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Enums.Notification;
using Core.Enums.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.IdentityServices.Enum;
using Services.IdentityServices.Interfaces;
using Services.Interfaces;
using Services.Interfaces.Notifications;
using Services.Model.Account;
using Services.Model.AdminSection.Admin;
using Services.Model.AdminSection.User;
using Services.Model.Pagination;
using WebApi.Controllers;
using WebApi.Infractructure.Response;

namespace WebApi.Areas.Admin
{
    /// <summary>
    /// Manage admins from admin panel
    /// </summary>
    /// <seealso cref="BaseApiController" />
    [Authorize(Policy = "AdministratorsOnly")]
    [Route("admin/manage")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public class AdminManageController : BaseApiController
	{
		private readonly IAppUserManager _userManager;
	    private readonly IUserService _userService;
        private readonly IQueueMessageService _messageService;
	    private readonly IEmailNotificationService _emailNotificationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminManageController" /> class.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="messageService">The message service</param>
        /// <param name="userService">The user service</param>
        /// <param name="emailNotificationService">Email notification service</param>        
        public AdminManageController(IAppUserManager userManager, IQueueMessageService messageService, IUserService userService, IEmailNotificationService emailNotificationService)
		{
			_userManager = userManager;
            _messageService = messageService;
		    _userService = userService;
		    _emailNotificationService = emailNotificationService;
		}

        /// <summary>
        /// Logins the specified admin.
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
        public async Task<LoginResponseModel> Login([FromBody] AdminLoginModel model)
        {
            return await _userManager.LoginAsync(new LoginRequestModel {Email = model.Email, Password = model.Password});
        }

        /// <summary>
        /// Gets admin list.
        /// </summary>
        /// <param name="pageNumber">The offset.</param>
        /// <param name="pageSize">The count.</param>
        /// <param name="orderBy">The order by.</param>
        /// <returns>list of admins</returns>
        /// <response code="200">Success</response>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(List<AdminPreviewModel>), 200)]
        [Authorize(Policy = PermissionString.ViewAdmin)]
        public PaginationResult<AdminPreviewModel> GetAdmins(int pageNumber = PageNumber, int pageSize = PageSize, UserOrderBy orderBy = UserOrderBy.Name)
        {
            return _userService.GetAdmins(pageNumber, pageSize, orderBy);
        }

        /// <summary>
        /// Gets the blocked admins.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("blocked")]
        [ProducesResponseType(typeof(ResponseWrapper<List<AdminPreviewModel>>), 200)]
        [Authorize(Policy = PermissionString.ViewBlockedAdmin)]
        public async Task<List<AdminPreviewModel>> GetBlockedAdmins(int? offset = null, int? count = null)
        {
            return await _userManager.GetPreviewListAsync<AdminPreviewModel>(count ?? PageSize, offset ?? PageNumber, user => user.Id != UserId && !user.IsActive && user.IsAdmin);
        }

        /// <summary>
        /// Gets the specified admin info.
        /// </summary>
        /// <param name="id">Admin Id.</param>
        /// <returns>Admin info</returns>
        /// <response code="200">Success</response>
        /// <response code="404">Admin not found</response>
        [HttpGet]
		[Route("{id:long}")]
        [ProducesResponseType(typeof(ResponseWrapper<AdminProfileModel>), 200)]
		public async Task<AdminProfileModel> Get(long id)
		{
			return await _userManager.GetProfileInfoAsync<AdminProfileModel>(id);
		}

		/// <summary>
		/// Add new admin.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <response code="200">Success</response>
		/// <response code="409">Admin with specific email already exist</response>
		/// <response code="409">Identity errors with model</response>
		/// <response code="409">Model validation errors</response>
		[HttpPost]
		[Route("add")]
		[AllowAnonymous]
		[ProducesResponseType(typeof(ResponseWrapper<object>), 200)]
        [Authorize(Policy = PermissionString.CreateAdmin)]
        public async Task<OkResult> Post([FromBody]AdminCreateModel model)
		{
			var callback = await _userManager.AddAdminAsync(model);
		    try
		    {
		        await _emailNotificationService.SendMessageAsync(callback.Body, callback.Subject, callback.User.Id, ActivityType.AdminAdded);
		    }
		    catch (Exception) { }
            return Ok();
		}

	    ///// <summary>
	    ///// Create new admin.
	    ///// </summary>
	    ///// <param name="model">The model.</param>
	    ///// <response code="200">Success</response>
	    ///// <response code="409">Admin with specific email already exist</response>
	    ///// <response code="409">Identity errors with model</response>
	    ///// <response code="409">Model validation errors</response>
	    //[HttpPost]
	    //[Route("create")]
	    //[AllowAnonymous]
	    //[ProducesResponseType(typeof(ResponseWrapper<SuccessModel>), 200)]
	    //public async Task<SuccessModel> CreateAdmin([FromBody]NewAdminModel model)
	    //{
	    //    await _userManager.CreateAdminAsync(model);
	    //    return new SuccessModel();
	    //}

        /// <summary>
        /// Update admin profile info.
        /// </summary>
        /// <param name="id">Admin Id.</param>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [HttpPut]
		[Route("{id:long}")]
		[ProducesResponseType(typeof(ResponseWrapper<object>), 200)]
        [Authorize(Policy = PermissionString.UpdateAdmin)]
        public async Task<OkResult> Put(long id, [FromBody]AdminProfileModel model)
		{
			await _userManager.UpdateProfileInfoAsync(model, id);
			return Ok();
		}

		/// <summary>
		/// Blocks the specified admin.
		/// </summary>
		/// <param name="id">Admin id.</param>
		/// <response code="200">Success</response>
		/// <response code="404">User not found</response>
		[HttpPut]
		[Route("{id:long}/block")]
		[ProducesResponseType(typeof(ResponseWrapper<List<object>>), 200)]
        [Authorize(Policy = PermissionString.BlockAdmin)]
        public async Task<OkResult> Block(long id)
		{
			await _userManager.SetScreenStatusAsync(id, false);

			return Ok();
		}

		/// <summary>
		/// Unblocks the specified admin.
		/// </summary>
		/// <param name="id">Admin id.</param>
		/// <response code="200">Success</response>
		/// <response code="404">Admin not found</response>
		[HttpPut]
		[Route("blocked/{id:long}/unblock")]
		[ProducesResponseType(typeof(ResponseWrapper<object>), 200)]
        [Authorize(Policy = PermissionString.UnBlockAdmin)]
        public async Task<OkResult> Unlock(long id)
		{
			await _userManager.SetScreenStatusAsync(id, true);

			return Ok();
		}

        /// <summary>
		/// Changes the admin's password.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <response code="200">Success</response>
		/// <response code="409">Identity errors with model</response>
		/// <response code="409">Model validation errors</response>
		[Authorize]
        [HttpPost]
        [Route("change-password")]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<IActionResult> ChangePassword([FromBody]ChangeAdminPasswordModel model)
        {
            var callback = await _userManager.ChangeAdminPasswordAsync(model, model.UserId);
            await _messageService.AddChangePasswordMessageAsync(callback);

            return Ok();
        }

    }
}
