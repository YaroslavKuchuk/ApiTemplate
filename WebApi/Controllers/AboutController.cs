using System;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    /// <summary>
    /// Show specific version of API
    /// </summary>
    /// <seealso cref="Controller" />
    [AllowAnonymous]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/[controller]")]
	public class AboutController : Controller
	{
		/// <summary>
		/// Gets this instance.
		/// </summary>
		/// <returns>Build number</returns>
		[HttpGet]
		[Route("")]
		public int Get()
		{
			Version version = Assembly.GetExecutingAssembly().GetName().Version;
			var build = version.Build;
			return build;
		}
	}
}