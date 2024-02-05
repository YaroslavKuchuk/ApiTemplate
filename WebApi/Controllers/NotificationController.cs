using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Model;
using Services.Model.Notifications;
using WebApi.Infractructure.Response;

namespace WebApi.Controllers
{
    //[AllowAnonymous]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class NotificationController : BaseApiController
    {
        private readonly IMessageHistoryService _messageHistoryService;

        public NotificationController(IMessageHistoryService messageHistoryService)
        {
            _messageHistoryService = messageHistoryService;
        }

        /// <summary>
        /// Endpoint that triggers resend notification logic
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("resend")]
        public async Task<SuccessModel> Get()
        {
            await _messageHistoryService.ResendNotifications();
            return new SuccessModel();
        }

        /// <summary>
        /// Get user's notification list.
        /// </summary>
        /// <returns>List of notifications</returns>
        /// <response code="200">Success</response>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(ResponseWrapper<List<NotificationModel>>), 200)]
        public async Task<List<NotificationModel>> GetUserNotificationsList()
        {
            return await _messageHistoryService.GetNotifications(UserId);
        }

        /// <summary>
        /// Mark notification as read.
        /// </summary>
        /// <response code="200">Success</response>
        [HttpPost]
        [Route("mark-as-read")]
        [ProducesResponseType(typeof(ResponseWrapper<SuccessModel>), 200)]
        public async Task<SuccessModel> MarkNotificationsAsRead(string ids)
        {
            await _messageHistoryService.MarkNotificationsAsRead(UserId, ids);
            return new SuccessModel();
        }

        /// <summary>
        /// Delete notification.
        /// </summary>
        /// <param name="id">The notification id</param>
        /// <response code="200">Success</response>
        [HttpDelete]
        [Route("{id:long}")]
        [ProducesResponseType(typeof(ResponseWrapper<SuccessModel>), 200)]
        public async Task<SuccessModel> DeleteNotification(long id)
        {
            await _messageHistoryService.DeleteNotification(id);
            return new SuccessModel();
        }
    }
}