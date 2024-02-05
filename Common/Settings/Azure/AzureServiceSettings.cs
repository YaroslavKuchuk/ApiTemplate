using System;
using Common.SettingsReflector;

namespace Common.Settings.Azure
{
    public class AzureServiceSettings
    {
        [SettingsProperty("AzureAccountName")]
        public string AccountName { get; set; }

        [SettingsProperty("AzureAccountKey")]
        public string AccountKey { get; set; }

        public Uri StorageUrl => new Uri($"https://{AccountName.ToLowerInvariant()}.blob.core.windows.net/");

        [SettingsProperty("AzureContainerName")]
        public string ContainerName { get; set; }

        [SettingsProperty("AzureCdnName")]
        public string CdnName { get; set; }
    }
}
