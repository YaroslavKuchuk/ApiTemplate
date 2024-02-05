using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Services.Model.Account;
using Common.Settings.NotificationSettings;

namespace Services.IdentityServices.IdentityHelpers
{
	public class FacebookHelper
	{
		/// <summary>
		/// Gets the fb user.
		/// </summary>
		public static async Task<FacebookModel> GetFbUserAsync(string accessToken, FacebookSettings fbSettings)
		{
			dynamic fbUser = null;
			if (!string.IsNullOrEmpty(accessToken))
			{
				var fbClient = new Facebook.FacebookClient(accessToken) // create FB client based on appId and appSecret
				{
					AppId = fbSettings.ApplicationId,
					AppSecret = fbSettings.SecretKey
                };

				fbUser = await fbClient.GetTaskAsync("me/?fields=id,email,name,last_name, first_name,picture");
			}
			string json = JsonConvert.SerializeObject(fbUser);

			JObject jo = JObject.Parse(json);
			FacebookModel model = jo.ToObject<FacebookModel>();

		    model.Image = $"https://graph.facebook.com/{model.Id}/picture?width=500&height=500";
		    model.UserName = !string.IsNullOrWhiteSpace(model.Email) ? model.Email : $"{model.Name.Replace(" ", "_")}";

            return model;
		}		
	}
}