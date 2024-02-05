using System;
using System.Collections.Generic;
using AutoMapper;
using Core.Entities;
using Newtonsoft.Json;
using Services.Model.Messages;

namespace Services.Model.Notifications
{
    public class NotificationModel
    {
        public long Id { get; set; }
        public int ActivityType { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public List<FormattedMessageModel> FormattedMessage { get; set; }
        public Dictionary<object, object> ExtraData { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public class NotificationProfile : Profile
    {
        public NotificationProfile()
        {
            CreateMap<MessageHistory, NotificationModel>()
                .ForMember(m => m.CreateDate, opt => opt.MapFrom(x => x.UpdateDate))
                .ForMember(m => m.ExtraData, opt => opt.MapFrom(x => JsonConvert.DeserializeObject(x.Payload)))
                .ForMember(m => m.FormattedMessage, opt => opt.MapFrom(x=>!string.IsNullOrWhiteSpace(x.FormattedMessage) ? JsonConvert.DeserializeObject<List<FormattedMessageModel>>(x.FormattedMessage) : new List<FormattedMessageModel>()));
        }
    }
}
