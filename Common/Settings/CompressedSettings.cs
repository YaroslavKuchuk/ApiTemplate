using Common.SettingsReflector;

namespace Common.Settings
{
    public class CompressedSettings
	{
		[SettingsProperty("MaxSizeUncompressedInBytes")]
		public long UncompressedSizeInBytes { get; set; }
	}
}
