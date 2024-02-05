using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.Entities;
using Core.Enums;

namespace Services.Interfaces
{
    public interface IUserDeviceService
    {
        Task<UserDevice> FindBySingle(Expression<Func<UserDevice, bool>> predicate);
        Task<UserDevice> AddDevice(string deviceId, string deviceToken, OsType osType);
        Task<bool> IsDeviceExists(string deviceId);
        Task<UserDevice> SetDeviceToken(UserDevice device, string token, bool applyEmpty = true);
        Task<List<UserDevice>> GetAll();
    }
}