using System;

namespace Common.Helpers
{
	public class LogUtility
	{
		/// <summary>
		/// This methods formats an error message so that it is 
		/// in a nice format for the event log or other places
		/// </summary>
		/// <param name="exception">The exception</param>
		/// <returns>A formatted error message</returns>
		public static string BuildExceptionMessage(Exception exception)
		{
			Exception logException = exception;

			if (exception.InnerException != null)
			{
				logException = exception.InnerException;
			}
            string strErrorMsg="";
            /*string strErrorMsg = Environment.NewLine + "Error in Path :" + (System.Web.HttpContext.Current != null ? System.Web.HttpContext.Current.Request.Path : string.Empty);

            strErrorMsg += Environment.NewLine + "Raw Url :" + (System.Web.HttpContext.Current != null ? System.Web.HttpContext.Current.Request.RawUrl : string.Empty);

			strErrorMsg += Environment.NewLine + "Message :" + logException.Message;

			strErrorMsg += Environment.NewLine + "Source :" + logException.Source;

			strErrorMsg += Environment.NewLine + "Stack Trace :" + logException.StackTrace;

			strErrorMsg += Environment.NewLine + "TargetSite :" + logException.TargetSite;*/

            return strErrorMsg;
		}
	}
}
