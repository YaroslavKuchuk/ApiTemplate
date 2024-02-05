using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Enums.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.IdentityServices.Interfaces;
using Services.Interfaces;
using Services.Model.AdminSection.Admin;
using Services.Model.AdminSection.User;
using Services.Model.Pagination;
using Services.Model.User;
using WebApi.Controllers;
using WebApi.Infractructure.Response;

namespace WebApi.Areas.Admin
{
    /// <summary>
    /// Manage users from admin panel
    /// </summary>
    /// <seealso cref="BaseApiController" />
    [Authorize(Policy = "AdministratorsOnly")]
    [Route("admin/users")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public class AdminUsersController : BaseApiController
	{
		private readonly IAppUserManager _userManager;
        private readonly IUserService _userService;

	    /// <summary>
	    /// Initializes a new instance of the <see cref="AdminUsersController"/> class.
	    /// </summary>
	    /// <param name="userManager">The user manager.</param>
	    /// <param name="userService">The user service</param>
	    public AdminUsersController(IAppUserManager userManager, IUserService userService)
		{
			_userManager = userManager;
            _userService = userService;
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
		[Route("")]
		[ProducesResponseType(typeof(ResponseWrapper<PaginationResult<AdminPreviewModel>>), 200)]
		public PaginationResult<AdminPreviewModel> GetUsers(string searchQuery=null, int pageNumber = PageNumber, int pageSize = PageSize, UserOrderBy orderBy = UserOrderBy.Name)
		{
            return _userService.GetUsers(searchQuery, pageNumber, pageSize, orderBy);
        }


        /// <summary>
        /// Gets the blocked users.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("blocked")]
        [ProducesResponseType(typeof(List<AdminUserPreviewModel>), 200)]
        public async Task<List<AdminUserPreviewModel>> GetBlockedUsers(int? offset = null, int? count = null)
        {
            return await _userManager.GetPreviewListAsync<AdminUserPreviewModel>(count ?? PageSize, offset ?? PageNumber, user => !user.IsActive && !user.IsAdmin);
        }

        /// <summary>
        /// Gets the specified user.
        /// </summary>
        /// <param name="id">User id.</param>
        /// <returns>User full info</returns>
        /// <response code="200">Success</response>
        /// <response code="404">User not found</response>
        [HttpGet]
		[Route("{id:long}")]
		[ProducesResponseType(typeof(ResponseWrapper<UserProfileModel>), 200)]
		public async Task<UserProfileModel> Get(long id)
		{
			return await _userManager.GetProfileInfoAsync<UserProfileModel>(id);
		}

	    /// <summary>
	    /// Export user's data.
	    /// </summary>
	    /// <returns>List of users</returns>
	    /// <response code="200">Success</response>
	    [HttpGet]
	    [Route("extract-data")]
	    [ProducesResponseType(typeof(ResponseWrapper<ExtractDataUserModel>), 200)]
	    public async Task<ExtractDataUserModel> ExtractUsersData()
	    {
	        return await _userService.ExtractUsersData();
	    }

        /// <summary>
        /// Blocks the specified user.
        /// </summary>
        /// <param name="id">User id.</param>
        /// <response code="200">Success</response>
        /// <response code="404">User not found</response>
        [HttpPut]
		[Route("{id:long}/block")]
		[ProducesResponseType(typeof(ResponseWrapper<object>), 200)]
		public async Task<OkResult> Block(long id)
		{
			await _userManager.SetScreenStatusAsync(id, false);

			return Ok();
		}

		/// <summary>
		/// Unblocks the specified user.
		/// </summary>
		/// <param name="id">User id.</param>
		/// <response code="200">Success</response>
		/// <response code="404">User not found</response>
		[HttpPut]
		[Route("blocked/{id:long}/unblock")]
		[ProducesResponseType(typeof(ResponseWrapper<object>), 200)]
		public async Task<OkResult> Unlock(long id)
		{
			await _userManager.SetScreenStatusAsync(id, true);

			return Ok();
		}

        /// <summary>
		/// Update the specified user info.
		/// </summary>
		/// <response code="200">Success</response>
		/// <response code="409">Identity errors with model</response>
		/// <response code="409">Model validation errors</response>
		[HttpPut]
        [Route("{id:long}")]
        [ProducesResponseType(typeof(UserProfileModel), 200)]
        public async Task<UserProfileModel> Put([FromBody]UpdateProfileRequestModel model, long id)
        {
            return await _userManager.UpdateProfileInfoAsync(model, id);
        }

        /// <summary>
		/// Add new user.
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

        public async Task<OkResult> AddUser([FromBody]CreateUserAdminModel model)
        {
            await _userManager.CreateUserAsync(model);
            return Ok();
        }

        /// <summary>
        /// Delete user.
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="409">Admin with specific email already exist</response>
        /// <response code="409">Identity errors with model</response>
        /// <response code="409">Model validation errors</response>
        [HttpDelete]
        [Route("delete/{id:long}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ResponseWrapper<object>), 200)]
        public async Task<OkResult> DeleteUser(long id)
        {
            await _userManager.DeleteUser(id);
            return Ok();
        }
    }
}
