using System.Net;
using System.Runtime.Serialization;

namespace WebApi.Infractructure.Response
{
	/// <summary>
	/// Custom response wrapper for requests
	/// </summary>
	[DataContract]
	public class ApiResponse
	{
		/// <summary>
		/// Gets or sets the status code.
		/// </summary>
		/// <value>
		/// The status code.
		/// </value>
		[DataMember]
		public int StatusCode { get; set; }

		/// <summary>
		/// Gets or sets the status code.
		/// </summary>
		/// <value>
		/// The status code.
		/// </value>
		[DataMember]
		public bool Success { get; set; } = true;

		/// <summary>
		/// Gets or sets the error message.
		/// </summary>
		/// <value>
		/// The error message.
		/// </value>
		[DataMember(EmitDefaultValue = true)]
		public string Message { get; set; }

		/// <summary>
		/// Gets or sets the data.
		/// </summary>
		/// <value>
		/// The data.
		/// </value>
		[DataMember(EmitDefaultValue = true)]
		public object Data { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ApiResponse"/> class.
		/// </summary>
		public ApiResponse()
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ApiResponse" /> class.
		/// </summary>
		/// <param name="isSuccess">if set to <c>true</c> [is success].</param>
		/// <param name="statusCode">The status code.</param>
		/// <param name="data">The data.</param>
		/// <param name="errorMessage">The error message.</param>
		public ApiResponse(bool isSuccess, HttpStatusCode statusCode, object data = null, string errorMessage = null)
		{
			Success = isSuccess;
			StatusCode = (int)statusCode;
		    Data = isSuccess ? data : null;
            Message = errorMessage;
		}
	}
}