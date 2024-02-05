using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace WebApi.Controllers
{
    [AllowAnonymous]
    [Route("[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class InviteController : Controller
    {
        private readonly ISettingService _settingsService;

        public InviteController(ISettingService settingsService)
        {
            _settingsService = settingsService;
        }

        /// <summary>
        /// Invite friend.
        /// </summary>
        ///  /// <param name="id">The friend cipher id to share.</param>
        /// <returns>Redirect to app or AppStore</returns>
        [HttpGet]
        public async Task<ViewResult> Invite(string id)
        {
            var model = await _settingsService.GetInvitePage(id);
            return View(model);
        }
    }
}