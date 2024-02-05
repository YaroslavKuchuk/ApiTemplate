using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Common.Exceptions.Account;
using Core;
using Core.Data;
using Core.Data.Repositories;
using Core.Entities;
using Core.Enums.Orders;
using Core.Extensions;
using Microsoft.EntityFrameworkCore;
using Resources;
using Services.Interfaces;
using Services.Model.AdminSection.Admin;
using Services.Model.AdminSection.ExportImport;
using Services.Model.AdminSection.User;
using Services.Model.Pagination;
using Services.Model.User;
using Services.Specifications;

namespace Services.Implementation
{
    public class UserService: IUserService
    {
        private readonly IRepository<User> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = unitOfWork.Repository<User>();
        }


        public async Task<User> FindByFBIdAsync(string facebookId)
        {
            var users = await _repository.FindByAsync(u => u.FbToken == facebookId);
            User user = null;
            if (users.Count > 0)
            {
                user = users[0];
            }
            return user;
        }

        public async Task<User> FindByIdAsync(long userId)
        {
            return await _repository.GetSingleAsync(userId);
        }

        public async Task<User> FindByEmailAsync(string email)
        {
            var users = await _repository.FindByAsync(u => u.Email == email);
            return users.Any() ? users.FirstOrDefault() : null;
        }

        public async Task<User> FindByIdIncludingAsync(long userId, params Expression<Func<User, object>>[] includeProperties)
        {
            return await _repository.GetAllQueryable().Include(u => u.UserTokens).ThenInclude(t => t.UserDevice).FirstOrDefaultAsync(x=>x.Id == userId);
        }

        public async Task<User> FindByIdIncludingInfoAsync(long userId, params Expression<Func<User, object>>[] includeProperties)
        {
            return await _repository.GetSingleIncludingAsync(userId, includeProperties);
        }


        public async Task<bool> IsEmailUnique(string email)
        {
            return !await _repository.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> IsPhoneUnique(string phoneNumber)
        {
            return !await _repository.AnyAsync(u => u.PhoneNumber == phoneNumber);
        }

        public async Task<PaginationResult<SearchUserModel>> GetAppUsers(long currentUserId, string searchQuery, int pageNumber, int pageSize, UserOrderBy orderBy)
        {
            var users = await _repository.QueryBySpecificationList(new ISpecification<User>[] { new SearchUsersSpecification(searchQuery, currentUserId) }).SetOrder(orderBy).ToListAsync();
            var totalCount = users.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            var usersModel = Mapper.Map<List<SearchUserModel>>(users);
            return new PaginationResult<SearchUserModel>(pageNumber, pageSize, totalCount, totalPages, usersModel);
        }

        public async Task ChangeUserUpdateDate(long userId)
        {
            var user = await _repository.GetSingleAsync(userId);
            if (user == null || user.IsDelete)
            {
                throw new UserNotFoundException(Account.UserSpecificNotFound);
            }

            user.UpdateDate = DateTime.UtcNow;

            _repository.Update(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateUserSettingsAsync(long userId, UserSettingsModel model)
        {
            var user = await _repository.GetSingleAsync(userId);
            if (user == null || user.IsDelete)
            {
                throw new UserNotFoundException(Account.UserSpecificNotFound);
            }
            Mapper.Map(model, user);
            _repository.Update(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DisableNotificationWhenLoggedOut(long userId, UserSettingsModel model)
        {
            var user = await _repository.GetSingleAsync(userId);
            if (user == null || user.IsDelete)
            {
                throw new UserNotFoundException(Account.UserSpecificNotFound);
            }
            var oldEnablePushStatus = user.EnablePush;
            Mapper.Map(model, user);
            user.EnableNotifications = oldEnablePushStatus;
            _repository.Update(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public PaginationResult<AdminPreviewModel> GetAdmins(int pageNumber, int pageSize, UserOrderBy orderBy)
        {
            var admins = _repository.QueryBySpecificationList(new ISpecification<User>[] {new AdminUserSpecification()}).SetOrder(orderBy);
            return new PaginationResult<AdminPreviewModel>(pageNumber, pageSize, admins);
        }

        public PaginationResult<AdminPreviewModel> GetUsers(string searchQuery, int pageNumber, int pageSize, UserOrderBy orderBy)
        {
            var users = _repository.QueryBySpecificationList(new ISpecification<User>[] { new UsersSpecification(searchQuery) }).SetOrder(orderBy);
            return new PaginationResult<AdminPreviewModel>(pageNumber, pageSize, users);
        }

        public List<UserExportModel> GetUsers(string searchQuery, UserOrderBy orderBy)
        {
            var users = _repository.QueryBySpecificationList(new ISpecification<User>[] { new UsersSpecification(searchQuery) }).SetOrder(orderBy);
            return Mapper.Map <List<UserExportModel>>(users.ToList());
        }

        public async Task<bool> DeleteUser(long userId)
        {
            var user = await _repository.GetSingleAsync(userId);
            if (user == null || user.IsDelete)
            {
                throw new UserNotFoundException(Account.UserSpecificNotFound);
            }
            var isNeedToCleanStorage = !user.IsAdmin;
            _repository.Delete(user);
            await _unitOfWork.SaveChangesAsync();
            //await _unitOfWork.DeleteUser(userId);

            return isNeedToCleanStorage;
        }

        public async Task<ExtractDataUserModel> ExtractUsersData()
        {
            var allUsers = await _repository.GetAllAsNoTrackingAsync();
            var users = allUsers.Where(u => !u.IsAdmin);
            var admins = allUsers.Where(u => u.IsAdmin);
            return new ExtractDataUserModel
            {
                Users = Mapper.Map<List<ExtractDataUserItemModel>>(users),
                Admins = Mapper.Map<List<ExtractDataUserItemModel>>(admins)
            };
        }
    }
}
