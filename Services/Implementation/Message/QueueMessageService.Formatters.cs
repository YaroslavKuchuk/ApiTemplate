using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Resources;
using Services.Model.Account;
using Services.Model.Messages;

namespace Services.Implementation.Message
{
    public partial class QueueMessageService
    {
		private static byte[] CreatePasswordMessage(SendPasswordCallback callback)
		{
			var message = new EmailMessage
			{
				To = callback.User.Email,
				Body = string.Format(callback.Body, callback.User.Email, callback.Password),
				Subject = callback.Subject
			};

			return GetMessageBytes(message);
		}

		private static byte[] CreateChangePasswordMessage(string to, string newPassword)
		{
			var message = new EmailMessage
			{
				To = to,
				Body = string.Format(EmailTemplates.ChangePasswordBody, to, newPassword),
				Subject = EmailTemplates.ChangePasswordSubject
			};

			return GetMessageBytes(message);
		}

		private static byte[] GetMessageBytes(object message)
		{
			using (var stream = new MemoryStream())
			{
				var formatter = new BinaryFormatter();
				formatter.Serialize(stream, message);

				return stream.ToArray();
			}
		}
	}
}
