using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Model.Invite;
using WebApi.Infractructure.Response;

namespace WebApi.Controllers
{
    /// <summary>
    /// get application related settings
    /// </summary>
    [Route("api/[controller]")]
    public class SettingsController : BaseApiController
    {
        private readonly ISettingService _settingsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsController"/> class.
        /// </summary>
        /// <param name="settingsService">The settings service.</param>
        public SettingsController(ISettingService settingsService)
        {
            _settingsService = settingsService;
        }

        /// <summary>
        /// Invite friend url's setting.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("invite")]
        [ProducesResponseType(typeof(ResponseWrapper<InviteLinkModel>), 200)]
        public async Task<InviteLinkModel> InviteSettings()
        {
            var model = await _settingsService.GetInviteSettings(UserId);
            var url = $"{Request.Scheme}://{Request.Host.Value}/invite?id={model.InviteLink}";
            return new InviteLinkModel { InviteUrl = url };
        }
    }
}
