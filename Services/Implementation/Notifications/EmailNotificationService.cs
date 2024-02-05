using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Common.Settings;
using Core.Data;
using Core.Data.Repositories;
using Core.Entities;
using Core.Enums.Notification;
using Resources;
using Services.Helpers;
using Services.Interfaces;
using Services.Interfaces.Notifications;

namespace Services.Implementation.Notifications
{
    public class EmailNotificationService: IEmailNotificationService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<MessageHistory> _notificationHistoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly AWSSESSettings _awsSesSettings;
        private readonly string AppEmail;
        private readonly string Charset;

        public EmailNotificationService(IUnitOfWork unitOfWork, ISettingService settingService)
        {
            _unitOfWork = unitOfWork;
            _userRepository = _unitOfWork.Repository<User>();
            _notificationHistoryRepository = _unitOfWork.Repository<MessageHistory>();
            _awsSesSettings = settingService.GetSettings<AWSSESSettings>().Result;
            AppEmail = _awsSesSettings.AppEmail;
            Charset = _awsSesSettings.Charset;
        }

        public async Task SendMessageToAdminsAsync(string body, string subject, ActivityType activityType, Dictionary<object, object> payload = null)
        {
            var admins = await _userRepository.FindByAsync(u => u.IsAdmin && u.IsActive && !u.IsDelete);
            foreach (var admin in admins)
            {
                await SendMessageAsync(body, subject, admin.Id, activityType, payload);
            }
        }

        public async Task SendMessageAsync(string body, string subject, long userId, ActivityType activityType, Dictionary<object, object> payload = null)
        {
            var now = DateTime.UtcNow;
            try
            {
                var user = await _userRepository.GetSingleAsync(userId);
                if (user == null || user.IsDelete)
                {
                    await _notificationHistoryRepository.Insert(new MessageHistory
                    {
                        Message = body,
                        Title = subject,
                        ActivityType = activityType,
                        Type = Core.Enums.Notification.NotificationType.Email,
                        MessageId = null,
                        TryCount = 0,
                        Status = NotificationStatus.NotSent,
                        ErrorDescription = Account.UserNotFound,
                        UserId = user.Id,
                        CreateDate = now,
                        UpdateDate = now
                    });
                    await _unitOfWork.SaveChangesAsync();
                    return;
                }
                var sendRequest = new SendEmailRequest
                {
                    Source = AppEmail,
                    Destination = new Destination
                    {
                        ToAddresses =
                            new List<string> { user.Email }
                    },
                    Message = new Amazon.SimpleEmail.Model.Message
                    {
                        Subject = new Content(subject),
                        Body = new Body
                        {
                            Html = new Content
                            {
                                Charset = Charset,
                                Data = body
                            }
                        }
                    }
                };
                var response = new SendEmailResponse();
                using (var client = new AmazonSimpleEmailServiceClient(new AWSServiceCredentials(_awsSesSettings.AccessKey, _awsSesSettings.SecretKey, ""), RegionEndpoint.USEast1))
                {
                    try
                    {
                        response = await client.SendEmailAsync(sendRequest);
                    }
                    catch (Exception ex)
                    {
                        await _notificationHistoryRepository.Insert(new MessageHistory
                        {
                            Message = body,
                            Title = subject,
                            ActivityType = activityType,
                            Type = Core.Enums.Notification.NotificationType.Email,
                            MessageId = null,
                            TryCount = 0,
                            Status = NotificationStatus.NotSent,
                            ErrorDescription = ex.Message,
                            UserId = user.Id,
                            CreateDate = now,
                            UpdateDate = now
                        });
                        await _unitOfWork.SaveChangesAsync();
                        return;
                    }
                }
                await _notificationHistoryRepository.Insert(new MessageHistory
                {
                    Message = body,
                    Title = subject,
                    ActivityType = activityType,
                    Type = Core.Enums.Notification.NotificationType.Email,
                    MessageId = response.MessageId,
                    TryCount = 0,
                    Status = NotificationStatus.Sent,
                    ErrorDescription = null,
                    UserId = user.Id,
                    CreateDate = now,
                    UpdateDate = now
                });
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _notificationHistoryRepository.Insert(new MessageHistory
                {
                    Message = body,
                    Title = subject,
                    ActivityType = activityType,
                    Type = Core.Enums.Notification.NotificationType.Email,
                    MessageId = null,
                    TryCount = 0,
                    Status = NotificationStatus.NotSent,
                    ErrorDescription = ex.Message,
                    UserId = userId,
                    CreateDate = now,
                    UpdateDate = now
                });
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task SendMessageAsync(string subject, string body, string emailAddress, ActivityType activityType)
        {
            var now = DateTime.UtcNow;
            try
            {                
                var sendRequest = new SendEmailRequest
                {
                    Source = AppEmail,
                    Destination = new Destination
                    {
                        ToAddresses =
                            new List<string> { emailAddress }
                    },
                    Message = new Amazon.SimpleEmail.Model.Message
                    {
                        Subject = new Content(subject),
                        Body = new Body
                        {
                            Html = new Content
                            {
                                Charset = Charset,
                                Data = body
                            }
                        }
                    }
                };
                var response = new SendEmailResponse();
                using (var client = new AmazonSimpleEmailServiceClient(new AWSServiceCredentials(_awsSesSettings.AccessKey, _awsSesSettings.SecretKey, ""), RegionEndpoint.USEast1))
                {
                    try
                    {
                        response = await client.SendEmailAsync(sendRequest);
                    }
                    catch (Exception ex)
                    {
                        await _notificationHistoryRepository.Insert(new MessageHistory
                        {
                            Message = body,
                            Title = subject,
                            ActivityType = activityType,
                            Type = Core.Enums.Notification.NotificationType.Email,
                            MessageId = null,
                            TryCount = 0,
                            Status = NotificationStatus.NotSent,
                            ErrorDescription = ex.Message,
                            UserId = null,
                            CreateDate = now,
                            UpdateDate = now
                        });
                        await _unitOfWork.SaveChangesAsync();
                        return;
                    }
                }
                await _notificationHistoryRepository.Insert(new MessageHistory
                {
                    Message = body,
                    Title = subject,
                    ActivityType = activityType,
                    Type = Core.Enums.Notification.NotificationType.Email,
                    MessageId = response.MessageId,
                    TryCount = 0,
                    Status = NotificationStatus.Sent,
                    ErrorDescription = null,
                    UserId = null,
                    CreateDate = now,
                    UpdateDate = now
                });
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _notificationHistoryRepository.Insert(new MessageHistory
                {
                    Message = body,
                    Title = subject,
                    ActivityType = activityType,
                    Type = Core.Enums.Notification.NotificationType.Email,
                    MessageId = null,
                    TryCount = 0,
                    Status = NotificationStatus.NotSent,
                    ErrorDescription = ex.Message,
                    UserId = null,
                    CreateDate = now,
                    UpdateDate = now
                });
                await _unitOfWork.SaveChangesAsync();
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

                    var sendRequest = new SendEmailRequest
                    {
                        Source = AppEmail,
                        Destination = new Destination
                        {
                            ToAddresses =
                                new List<string> { user.Email }
                        },
                        Message = new Amazon.SimpleEmail.Model.Message
                        {
                            Subject = new Content(message.Title),
                            Body = new Body
                            {
                                Html = new Content
                                {
                                    Charset = Charset,
                                    Data = message.Message
                                }
                            }
                        },
                    };
                    SendEmailResponse response;
                    using (var client = new AmazonSimpleEmailServiceClient(new AWSServiceCredentials(_awsSesSettings.AccessKey, _awsSesSettings.SecretKey, ""), RegionEndpoint.USEast1))
                    {
                        try
                        {
                            response = await client.SendEmailAsync(sendRequest);
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
                    message.MessageId = response.MessageId;
                    message.Status = NotificationStatus.Sent;
                    message.UpdateDate = now;
                    _notificationHistoryRepository.Update(message);
                    await _unitOfWork.SaveChangesAsync();
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
    }
}
