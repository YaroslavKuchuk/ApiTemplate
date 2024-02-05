
using Common.SettingsReflector;

namespace Common.Settings
{
    public class ForgotPasswordSettings 
    {
        [SettingsProperty("ForgotPasswordWebLink")]
        public string ForgotPasswordWebPortalLink { get; set; }

        [SettingsProperty("ForgotPasswordDeltaExpDay")]
        public int ForgotPasswordDeltaExpDay { get; set; }
    }
}
