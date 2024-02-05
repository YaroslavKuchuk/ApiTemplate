using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.Data;
using Core.Data.Repositories;
using Core.Entities;
using Core.Enums;
using Services.Interfaces;

namespace Services.Implementation
{
    public class UserDeviceService : IUserDeviceService
    {
        private readonly IRepository<UserDevice> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public UserDeviceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = unitOfWork.Repository<UserDevice>();
        }

        public async Task<UserDevice> FindBySingle(Expression<Func<UserDevice, bool>> predicate)
        {
            var devices = await _repository.FindByAsync(predicate);
            return devices.FirstOrDefault();
        }

        public async Task<UserDevice> AddDevice(string deviceId, string deviceToken, OsType osType)
        {
            var devices = await _repository.FindByAsync(e => e.DeviceId.Equals(deviceId));;
            var device = devices.FirstOrDefault();

            if (device != null)
            {
                return device;
            }

            device = new UserDevice
            {
                DeviceId = deviceId,
                DevicePushToken = deviceToken,
                CreateDate =  DateTime.UtcNow,
                UpdateDate =  DateTime.UtcNow,
                IsEnabledPush = true,
                OsType = (short)osType,
            };

            await _repository.Insert(device);
            await _unitOfWork.SaveChangesAsync();

            return device;
        }

        public async Task<bool> IsDeviceExists(string deviceId)
        {
            var devices = await _repository.FindByAsync(e => e.DeviceId.Equals(deviceId));
            var device = devices.FirstOrDefault();

            return device != null;
        }

        public async Task<UserDevice> SetDeviceToken(UserDevice device, string token, bool applyEmpty = true)
        {
            if (!applyEmpty && string.IsNullOrWhiteSpace(token))
            {
                return device;
            }

            device.DevicePushToken = token;
            device.UpdateDate =  DateTime.UtcNow;
            _repository.Update(device);
            await _unitOfWork.SaveChangesAsync();

            return device;
        }

        public async Task<List<UserDevice>> GetAll()
        {
            return await _repository.GetAllAsNoTrackingAsync();
        }
    }
}
