using System.Threading.Tasks;
using Common.Settings;
using InstaSharp;
using Services.Model.Account;

namespace Services.IdentityServices.IdentityHelpers
{
    public static class InstagramHelper
    {
        public static async Task<InstagramModel> GetInstagramUserAsync(string code, InstagramSettings instagramSettings)
        {
            var config = new InstagramConfig(instagramSettings.ClientId, instagramSettings.ClientSecret, instagramSettings.RedirectUrl, "");
            //var authUrl = OAuth.AuthLink(config, new List<OAuth.Scope> { OAuth.Scope.Basic }, OAuth.ResponseType.Code);
            var auth = new OAuth(config);
            var oauthResponse = await auth.RequestToken(code);
            var apiUser = oauthResponse.User;
            var instagramModel = new InstagramModel();
            if (apiUser != null)
            {
                var fullNameSplited = apiUser.FullName.Split(' ');
                if (fullNameSplited.Length == 2)
                {
                    instagramModel.FirstName = fullNameSplited[0];
                    instagramModel.LastName = fullNameSplited[1];
                }
                else
                {
                    instagramModel.FirstName = apiUser.FullName;
                }
                instagramModel.Id = apiUser.Id;
                instagramModel.Username = apiUser.Username;
                instagramModel.ProfilePicture = apiUser.ProfilePicture;
            }

            return instagramModel;
        }
    }
}
