using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    /// <summary>
    /// Base controller
    /// </summary>
    /// <seealso cref="Controller" />
    [ApiController]
    public class BaseApiController : Controller
	{
		/// <summary>
		/// The page size
		/// </summary>
		protected const int PageSize = 10;
		/// <summary>|
		/// The offset
		/// </summary>
		protected const int PageNumber = 1;

	    /// <summary>
	    /// Gets the user identifier.
	    /// </summary>
	    /// <value>
	    /// The user identifier.
	    /// </value>
	    protected long UserId => Convert.ToInt64(Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "sid")?.Value);

	}
}