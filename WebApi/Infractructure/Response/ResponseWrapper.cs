using System.Runtime.Serialization;

namespace WebApi.Infractructure.Response
{
	public class ResponseWrapper<TData>
		where TData : new()
	{
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
		public TData Data { get; set; }

		public ResponseWrapper()
		{

		}

		public ResponseWrapper(string message)
		{
			Message = message;
		}

		public ResponseWrapper(TData data, string message = null)
			: this(message)
		{
			Data = data;
		}

		public ResponseWrapper(string message, TData data = default(TData)) : this(data, message)
		{
		}
	}
}