using System;
using AutoMapper;

namespace Services.Model.User
{
	/// <summary>
	/// Profile model for mobile app
	/// </summary>
	public class UserProfileModel
	{
	    public long Id { get; set; }
        /// <summary>
        /// User first name
        /// </summary>
        /// <value>
        /// The full name.
        /// </value>
        public string FirstName { get; set; }

	    /// <summary>
	    /// User last name
	    /// </summary>
	    /// <value>
	    /// The full name.
	    /// </value>
	    public string LastName { get; set; }

        /// <summary>
        /// User email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        public string Email { get; set; }

		/// <summary>
		/// User profile image.
		/// </summary>
		/// <value>
		/// The image.
		/// </value>
		public string Image { get; set; }

        public bool IsActive { get; set; }
	    public bool IsNotificationsEnabled { get; set; }
        public DateTime UpdateDate { get; set; }

    }

    public class UserProfileProfile : Profile
    {
        public UserProfileProfile()
        {
            CreateMap<Core.Entities.User, UserProfileModel>()
                .ForMember(m => m.Email, opt => opt.MapFrom(x => x.Email))
                .ForMember(m => m.FirstName, opt => opt.MapFrom(x => x.FirstName))
                .ForMember(m => m.LastName, opt => opt.MapFrom(x => x.LastName))
                .ForMember(m => m.Image, opt => opt.MapFrom(x => x.Image))
                .ForMember(m => m.IsNotificationsEnabled, opt => opt.MapFrom(x => x.EnablePush));
        }
    }
}
