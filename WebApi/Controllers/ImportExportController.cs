using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.IdentityServices.Enum;
using Services.Interfaces.ImportExportData;

namespace WebApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("upload/[controller]")]
    public class ImportExportController : BaseApiController
    {
        private readonly IImportExportService _importExportService;
        
        public ImportExportController(IImportExportService importExportService)
        {
            _importExportService = importExportService;
        }

        [HttpGet]
        [Route("users.csv")]
        [Produces("text/csv")]
        [Authorize(Policy = PermissionString.GetCsvReport)]
        public IActionResult GetUsersAsCsv()
        {
            return Ok(_importExportService.GetUsers(string.Empty));

        }
    }
}