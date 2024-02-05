using AutoMapper;

namespace Services.Model.Mappings
{
	/// <summary>
	/// Configrate automapping
	/// </summary>
	public class AutoMapperConfig
	{
		public static void Register()
		{
            Mapper.Initialize(c => c.AddProfiles("Services.Model"));
        }
	}
}