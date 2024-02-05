using Common.SettingsReflector;

namespace Common.Settings.Admin
{
    public class AdminLinkSetting
    {
        [SettingsProperty("AdminPanelLink")]
        public string Url { get; set; }
    }
}
