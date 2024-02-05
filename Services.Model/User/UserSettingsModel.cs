using System;
using AutoMapper;

namespace Services.Model.User
{
    public class UserSettingsModel
    {
        public bool EnablePush { get; set; }
    }

    public class UserSettingsProfile : Profile
    {
        public UserSettingsProfile()
        {
           CreateMap<UserSettingsModel, Core.Entities.User>()
                .ForMember(m => m.EnablePush, opt => opt.MapFrom(x => x.EnablePush))
                .ForMember(m => m.UpdateDate, opt => opt.MapFrom(x =>DateTime.UtcNow));
        }
    }
}
