using System;
using System.Net;

namespace Common.Exceptions
{
	public class ApiException : Exception
	{
		/// <summary>
		///     ctor
		/// </summary>
		/// <param name="errorMessage"></param>
		public ApiException(string errorMessage)
			: this(errorMessage, HttpStatusCode.Conflict)
		{
		}

		/// <summary>
		///     ctor
		/// </summary>
		/// <param name="errorMessage">Error message</param>
		/// <param name="httpStatusCode">Response http status code</param>
		public ApiException(String errorMessage, HttpStatusCode httpStatusCode)
            : base(errorMessage)
        {
			StatusCode = httpStatusCode;
		}

		/// <summary>
		///     Response http status code
		/// </summary>
		public HttpStatusCode StatusCode { get; set; }
	}
}