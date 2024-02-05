using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.Entities;
using Core.Enums.Orders;
using Services.Model.AdminSection.Admin;
using Services.Model.AdminSection.ExportImport;
using Services.Model.AdminSection.User;
using Services.Model.Pagination;
using Services.Model.User;

namespace Services.Interfaces
{
    public interface IUserService
    {
        Task<User> FindByFBIdAsync(string facebookId);
        Task<User> FindByIdAsync(long userId);
        Task<User> FindByEmailAsync(string email);
        Task<User> FindByIdIncludingAsync(long userId, params Expression<Func<User, object>>[] includeProperties);
        Task<User> FindByIdIncludingInfoAsync(long userId, params Expression<Func<User, object>>[] includeProperties);
        Task<bool> IsEmailUnique(string email);
        Task<bool> IsPhoneUnique(string phoneNumber);
        Task<PaginationResult<SearchUserModel>> GetAppUsers(long currentUserId, string searchQuery, int pageNumber, int pageSize, UserOrderBy orderBy);

        Task ChangeUserUpdateDate(long userId);
        Task UpdateUserSettingsAsync(long userId, UserSettingsModel model);
        Task DisableNotificationWhenLoggedOut(long userId, UserSettingsModel model);

        PaginationResult<AdminPreviewModel> GetAdmins(int pageNumber, int pageSize, UserOrderBy orderBy);
        PaginationResult<AdminPreviewModel> GetUsers(string searchQuery, int pageNumber, int pageSize, UserOrderBy orderBy);
        List<UserExportModel> GetUsers(string searchQuery, UserOrderBy orderBy);
        Task<bool> DeleteUser(long userId);

        Task<ExtractDataUserModel> ExtractUsersData();
    }
}
