using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Common.Settings;
using Core.Data;
using Core.Data.Repositories;
using Core.Entities;
using Core.Entities.Notification;
using Core.Enums;
using Core.Enums.Notification;
using Newtonsoft.Json;
using Resources;
using Services.Helpers;
using Services.Interfaces;
using Services.Interfaces.Notifications;
using Services.Model.Messages;

namespace Services.Implementation.Notifications
{
    public class PushNotificationService : IPushNotificationService
    {
        private readonly IRepository<AmazonTopic> _amazonTopicRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserToken> _userTokenRepository;
        private readonly IRepository<UserDevice> _userDeviceRepository;
        private readonly IRepository<MessageHistory> _notificationHistoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly AWSSNSSettings _awsSnsSettings;

        public PushNotificationService(ISettingService settingService, IUnitOfWork unitOfWork)
        {
            _awsSnsSettings = settingService.GetSettings<AWSSNSSettings>().Result;
            _unitOfWork = unitOfWork;
            _amazonTopicRepository = _unitOfWork.Repository<AmazonTopic>();
            _userRepository = _unitOfWork.Repository<User>();
            _userTokenRepository = _unitOfWork.Repository<UserToken>();
            _userDeviceRepository = _unitOfWork.Repository<UserDevice>();
            _notificationHistoryRepository = _unitOfWork.Repository<MessageHistory>();
        }

        public async Task SendMessageAsync(string message, string title, long userId, ActivityType activityType,
            string eventCode = null, Dictionary<object, object> payload = null,
            List<FormattedMessageModel> formattedMessage = null)
        {
            var now = DateTime.UtcNow;
            var errorMessage = new MessageHistory
            {
                Message = message,
                FormattedMessage = JsonConvert.SerializeObject(formattedMessage),
                ActivityType = activityType,
                Title = title,
                Type = NotificationType.Push,
                MessageId = null,
                TryCount = 0,
                Status = NotificationStatus.NotSent,
                CreateDate = now,
                UpdateDate = now,
                EventCode = eventCode,
                Payload = JsonConvert.SerializeObject(payload)
            };
            try
            {
                var user = await _userRepository.GetSingleAsync(userId);
                if (user == null || user.IsDelete)
                {
                    errorMessage.ErrorDescription = Account.UserNotFound;
                    await _notificationHistoryRepository.Insert(errorMessage);
                    await _unitOfWork.SaveChangesAsync();
                    return;
                }
                if (!user.EnablePush)
                {
                    return;
                }

                var userAuthToken = (await _userTokenRepository.GetFilteredIncludingAsync(x => x.UserId == userId && x.UserDevice.IsEnabledPush && !string.IsNullOrWhiteSpace(x.UserDevice.DevicePushToken),
                    i => i.UserDevice)).OrderByDescending(x => x.UserDevice.UpdateDate).FirstOrDefault();
                if (userAuthToken == null)
                {
                    errorMessage.UserId = user.Id;
                    errorMessage.ErrorDescription = Account.UserTokenNotFound;
                    await _notificationHistoryRepository.Insert(errorMessage);
                    await _unitOfWork.SaveChangesAsync();
                    return;
                }
                if (userAuthToken.UserDeviceId == null)
                {
                    errorMessage.UserId = user.Id;
                    errorMessage.ErrorDescription = Account.UserDeviceIdNotFound;
                    await _notificationHistoryRepository.Insert(errorMessage);
                    await _unitOfWork.SaveChangesAsync();
                    return;
                }
                var userDevice = userAuthToken.UserDevice;
                if (userDevice == null)
                {
                    errorMessage.UserId = user.Id;
                    errorMessage.ErrorDescription = Account.UserDeviceNotFound;
                    await _notificationHistoryRepository.Insert(errorMessage);
                    await _unitOfWork.SaveChangesAsync();
                    return;
                }

                var amazonTopic = (await _amazonTopicRepository.FindByAsync(x => x.DeviceToken == userDevice.DevicePushToken)).FirstOrDefault();
                if (amazonTopic == null)
                {
                    amazonTopic = await CreatePushEndpoint(userDevice.DevicePushToken, (OsType)userDevice.OsType);
                    if (amazonTopic == null)
                    {
                        errorMessage.UserId = user.Id;
                        errorMessage.ErrorDescription = Account.OsTypeInvalid;
                        await _notificationHistoryRepository.Insert(errorMessage);
                        await _unitOfWork.SaveChangesAsync();
                        return;
                    }
                }
                var androidData = new Dictionary<string, object>
                    {
                        {"title", title},
                        {"message", message},
                        {"code", eventCode},
                        {"payload", JsonConvert.SerializeObject(payload)}
                    };
                var iosAlert = new Dictionary<string, object> {
                        { "title", title },
                        { "body", message }
                    };
                var iosExtraData = new Dictionary<string, object>
                    {
                        {"code", eventCode},
                        {"pay", JsonConvert.SerializeObject(payload)}
                    };
                var unreadMessagesCount = user.UnreadMessageCount + 1;
                var iosData = new Dictionary<string, object>
                    {
                        {"alert", iosAlert},
                        {"badge", unreadMessagesCount},
                        {"sound", "default"},
                        {"data", iosExtraData}
                    };
                var jsonMessage = JsonConvert.SerializeObject(new Dictionary<string, string>
                    {
                        {
                            "default", message
                        },
                        {
                            "GCM", JsonConvert.SerializeObject(new Dictionary<string, object>
                            {
                                {
                                    "data", androidData
                                }
                            })
                        },
                        {
                            "APNS", JsonConvert.SerializeObject(new Dictionary<string, object>
                            {
                                {
                                    "aps", iosData
                                }
                            })
                        }
                    });

                var pushMsg = new PublishRequest
                {
                    Message = jsonMessage,
                    MessageStructure = "json",
                    TargetArn = amazonTopic.Arn
                };

                var result = new PublishResponse();
                using (var client = new AmazonSimpleNotificationServiceClient(new AWSServiceCredentials(_awsSnsSettings.AccessKey, _awsSnsSettings.SecretKey, ""), RegionEndpoint.USEast1))
                {
                    var response = await client.GetEndpointAttributesAsync(new GetEndpointAttributesRequest { EndpointArn = amazonTopic.Arn });
                    var isEnabled = string.Empty;
                    response.Attributes.TryGetValue("Enabled", out isEnabled);
                    if (isEnabled != "true")
                    {
                        errorMessage.UserId = user.Id;
                        errorMessage.ErrorDescription = Account.AwsEndpointIsNotEnabled;
                        await _notificationHistoryRepository.Insert(errorMessage);
                        await _unitOfWork.SaveChangesAsync();
                        return;
                    }

                    try
                    {
                        result = await client.PublishAsync(pushMsg);
                    }
                    catch (Exception ex)
                    {
                        errorMessage.UserId = user.Id;
                        errorMessage.ErrorDescription = ex.Message;
                        await _notificationHistoryRepository.Insert(errorMessage);
                        await _unitOfWork.SaveChangesAsync();
                        return;
                    }
                }
                await _notificationHistoryRepository.Insert(new MessageHistory
                {
                    Message = message,
                    FormattedMessage = JsonConvert.SerializeObject(formattedMessage),
                    ActivityType = activityType,
                    Title = title,
                    Type = NotificationType.Push,
                    MessageId = result.MessageId,
                    TryCount = 0,
                    Status = NotificationStatus.Sent,
                    ErrorDescription = null,
                    UserId = user.Id,
                    CreateDate = now,
                    UpdateDate = now,
                    EventCode = eventCode,
                    Payload = JsonConvert.SerializeObject(payload)
                });
                await _unitOfWork.SaveChangesAsync();
                await UpdateUserUnreadMessageCount(user);
            }
            catch (Exception ex)
            {
                errorMessage.UserId = userId;
                errorMessage.ErrorDescription = ex.Message;
                await _notificationHistoryRepository.Insert(errorMessage);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task SendMessageAsync(string message, string title, string phoneNumber, ActivityType activityType,
            string eventCode = null, Dictionary<object, object> payload = null,
            List<FormattedMessageModel> formattedMessage = null)
        {
            DateTime now = DateTime.UtcNow;
            MessageHistory notificationHistory = new MessageHistory()
            {
                Message = message,
                FormattedMessage = JsonConvert.SerializeObject(formattedMessage),
                ActivityType = activityType,
                Title = title,
                Type = NotificationType.Push,
                MessageId = null,
                TryCount = 0,
                Status = NotificationStatus.NotSent,
                CreateDate = now,
                UpdateDate = now,
                EventCode = eventCode,
                Payload = JsonConvert.SerializeObject(payload)
            };
            try
            {
                var client = new AmazonSimpleNotificationServiceClient(new AWSServiceCredentials(_awsSnsSettings.AccessKey, _awsSnsSettings.SecretKey, ""), RegionEndpoint.USEast1);

                PublishRequest pubRequest = new PublishRequest
                {
                    Message = message,
                    PhoneNumber = phoneNumber,
                    Subject = title

                };

                PublishResponse pubResponse = await client.PublishAsync(pubRequest);
                notificationHistory.Status = NotificationStatus.Sent;
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
                while (ex.InnerException != null)
                {
                    errorMessage += $" {ex.InnerException.Message}";
                    ex = ex.InnerException;
                }
                notificationHistory.Status = NotificationStatus.NotSent;
                notificationHistory.ErrorDescription = errorMessage;
            }
            finally
            {
                await _notificationHistoryRepository.Insert(notificationHistory);
                try
                {
                    await _unitOfWork.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
        }

        public async Task ResendMessageAsync(long messageHistoryId)
        {
            var message = await _notificationHistoryRepository.GetSingleAsync(messageHistoryId);
            if (message != null)
            {
                var now = DateTime.UtcNow;
                try
                {
                    var user = await _userRepository.GetSingleAsync(message.UserId.Value);
                    if (!message.UserId.HasValue || user == null || user.IsDelete)
                    {
                        message.Status = NotificationStatus.NotSent;
                        message.ErrorDescription = $"\r\n{message.TryCount}: {Account.UserNotFound}";
                        ++message.TryCount;
                        message.UpdateDate = now;
                        _notificationHistoryRepository.Update(message);
                        await _unitOfWork.SaveChangesAsync();
                        return;
                    }
                    var userAuthToken = (await _userTokenRepository.FindByAsync(x => x.UserId == message.UserId)).FirstOrDefault();
                    if (userAuthToken == null)
                    {
                        message.Status = NotificationStatus.NotSent;
                        message.ErrorDescription = $"\r\n{message.TryCount}: {Account.UserTokenNotFound}";
                        ++message.TryCount;
                        message.UpdateDate = now;
                        _notificationHistoryRepository.Update(message);
                        await _unitOfWork.SaveChangesAsync();
                        return;
                    }
                    if (userAuthToken.UserDeviceId == null)
                    {
                        message.Status = NotificationStatus.NotSent;
                        message.ErrorDescription = $"\r\n{message.TryCount}: {Account.UserDeviceIdNotFound}";
                        ++message.TryCount;
                        message.UpdateDate = now;
                        _notificationHistoryRepository.Update(message);
                        await _unitOfWork.SaveChangesAsync();
                        return;
                    }
                    var userDevice = await _userDeviceRepository.GetSingleAsync((long)userAuthToken.UserDeviceId);
                    if (userDevice == null)
                    {
                        message.Status = NotificationStatus.NotSent;
                        message.ErrorDescription = $"\r\n{message.TryCount}: {Account.UserDeviceNotFound}";
                        ++message.TryCount;
                        message.UpdateDate = now;
                        _notificationHistoryRepository.Update(message);
                        await _unitOfWork.SaveChangesAsync();
                        return;
                    }

                    var amazonTopic = (await _amazonTopicRepository.FindByAsync(x => x.DeviceToken == userDevice.DevicePushToken)).FirstOrDefault();
                    if (amazonTopic == null)
                    {
                        amazonTopic = await CreatePushEndpoint(userDevice.DevicePushToken, (OsType)userDevice.OsType);
                        if (amazonTopic == null)
                        {
                            message.Status = NotificationStatus.NotSent;
                            message.ErrorDescription = $"\r\n{message.TryCount}: {Account.OsTypeInvalid}";
                            ++message.TryCount;
                            message.UpdateDate = now;
                            _notificationHistoryRepository.Update(message);
                            await _unitOfWork.SaveChangesAsync();
                            return;
                        }
                    }
                    var androidData = new Dictionary<string, object>
                        {
                            {"title", message.Title},
                            {"message", message.Message},
                            {"code", message.EventCode},
                            {"payload", message.Payload}
                        };
                    var iosAlert = new Dictionary<string, object> {
                            { "title", message.Title },
                            { "body", message.Message }
                        };
                    var iosExtraData = new Dictionary<string, object>
                        {
                            {"code", message.EventCode},
                            {"pay", message.Payload}
                        };
                    var unreadMessagesCount = user.UnreadMessageCount + 1;
                    var iosData = new Dictionary<string, object>
                        {
                            {"alert", iosAlert},
                            {"badge", unreadMessagesCount},
                            {"sound", "default"},
                            {"data", iosExtraData}
                        };
                    var jsonMessage = JsonConvert.SerializeObject(new Dictionary<string, string>
                        {
                            {
                                "default", message.Message
                            },
                            {
                                "GCM", JsonConvert.SerializeObject(new Dictionary<string, object>
                                {
                                    {
                                        "data", androidData
                                    }
                                })
                            },
                            {
                                "APNS", JsonConvert.SerializeObject(new Dictionary<string, object>
                                {
                                    {
                                        "aps", iosData
                                    }
                                })
                            }
                        });

                    var pushMsg = new PublishRequest
                    {
                        Message = jsonMessage,
                        MessageStructure = "json",
                        TargetArn = amazonTopic.Arn
                    };
                    var result = new PublishResponse();

                    using (var client = new AmazonSimpleNotificationServiceClient(new AWSServiceCredentials(_awsSnsSettings.AccessKey, _awsSnsSettings.SecretKey, ""), RegionEndpoint.USEast1))
                    {
                        var response = await client.GetEndpointAttributesAsync(new GetEndpointAttributesRequest { EndpointArn = amazonTopic.Arn });
                        var isEnabled = string.Empty;
                        response.Attributes.TryGetValue("Enabled", out isEnabled);
                        if (isEnabled != "true")
                        {
                            message.Status = NotificationStatus.NotSent;
                            message.ErrorDescription = $"\r\n{message.TryCount}: {Account.AwsEndpointIsNotEnabled}";
                            ++message.TryCount;
                            message.UpdateDate = now;
                            _notificationHistoryRepository.Update(message);
                            await _unitOfWork.SaveChangesAsync();
                            return;
                        }

                        try
                        {
                            result = await client.PublishAsync(pushMsg);
                        }
                        catch (Exception ex)
                        {
                            message.Status = NotificationStatus.NotSent;
                            message.ErrorDescription = $"\r\n{message.TryCount}: {ex.Message}";
                            ++message.TryCount;
                            message.UpdateDate = now;
                            _notificationHistoryRepository.Update(message);
                            await _unitOfWork.SaveChangesAsync();
                            return;
                        }
                    }

                    message.MessageId = result.MessageId;
                    message.Status = NotificationStatus.Sent;
                    message.UpdateDate = now;
                    _notificationHistoryRepository.Update(message);
                    await _unitOfWork.SaveChangesAsync();
                    await UpdateUserUnreadMessageCount(user);
                }
                catch (Exception ex)
                {
                    message.Status = NotificationStatus.NotSent;
                    message.ErrorDescription = $"\r\n{message.TryCount}: {ex.Message}";
                    ++message.TryCount;
                    message.UpdateDate = now;
                    _notificationHistoryRepository.Update(message);
                    await _unitOfWork.SaveChangesAsync();
                }
            }
        }

        private async Task<AmazonTopic> CreatePushEndpoint(string deviceToken, OsType osType)
        {
            var applicationArn = string.Empty;
            switch (osType)
            {
                case OsType.Android:
                    applicationArn = _awsSnsSettings.AmazonAndroidEndpointArn;
                    break;

                case OsType.Ios:
                    applicationArn = _awsSnsSettings.AmazonIoSEndpointArn;
                    break;
            }
            if (!string.IsNullOrEmpty(applicationArn))
            {
                var request = new CreatePlatformEndpointRequest
                {
                    PlatformApplicationArn = applicationArn,
                    Token = deviceToken
                };
                string topicArn;
                using (var client = new AmazonSimpleNotificationServiceClient(new AWSServiceCredentials(_awsSnsSettings.AccessKey, _awsSnsSettings.SecretKey, ""), RegionEndpoint.USEast1))
                {
                    var response = await client.CreatePlatformEndpointAsync(request);
                    topicArn = response.EndpointArn;
                }
                var now = DateTime.UtcNow;
                var amazonTopic = new AmazonTopic
                {
                    Arn = topicArn,
                    DeviceToken = deviceToken,
                    CreateDate = now,
                    UpdateDate = now
                };
                await _amazonTopicRepository.Insert(amazonTopic);
                await _unitOfWork.SaveChangesAsync();
                return amazonTopic;
            }
            return null;
        }

        private async Task UpdateUserUnreadMessageCount(User user)
        {
            user.UnreadMessageCount++;
            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
