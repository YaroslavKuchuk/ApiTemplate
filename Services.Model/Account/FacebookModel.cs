using System;
using AutoMapper;
using Newtonsoft.Json;

namespace Services.Model.Account
{
	[Serializable]
	public class FacebookModel : BaseAuthModel
	{
		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }
	    [JsonProperty("first_name")]
	    public string FirstName { get; set; }

	    [JsonProperty("last_name")]
	    public string LastName { get; set; }

        [JsonProperty("email")]
		public string Email { get; set; }
		
		public string Image { get; set; }

        public string UserName { get; set; }
	}

    /// <summary>
    /// Creates the mappings.
    /// </summary>
    public class FacebookProfile : Profile
    {
        public FacebookProfile()
        {
            CreateMap<FacebookModel, Core.Entities.User>()
                .ForMember(m => m.Id, opt => opt.Ignore())
                .ForMember(m => m.Email, opt => opt.MapFrom(x => x.Email))
                .ForMember(m => m.UserName, opt => opt.MapFrom(x => x.UserName))
                .ForMember(m => m.FirstName, opt => opt.MapFrom(x => x.FirstName))
                .ForMember(m => m.LastName, opt => opt.MapFrom(x => x.LastName))
                .ForMember(m => m.Image, opt => opt.MapFrom(x => x.Image))
                .ForMember(m => m.UpdateDate, opt => opt.MapFrom(src =>DateTime.UtcNow))
                .ForMember(m => m.CreateDate, opt => opt.MapFrom(src =>DateTime.UtcNow))
                .ForMember(m => m.IsActive, opt => opt.MapFrom(src =>true))
                .ForMember(m => m.IsAdmin, opt => opt.MapFrom(src =>false));
        }
    }
}
