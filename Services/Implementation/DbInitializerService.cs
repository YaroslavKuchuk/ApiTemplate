using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Common.Helpers;
using Core.Data;
using Core.Data.Repositories;
using Core.Entities;
using Core.IdentityEntities;
using Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Services.IdentityServices.Enum;
using Services.Interfaces;
using Services.Model.AdminSection.Admin;

namespace Services.Implementation
{
    public class DbInitializerService : IDbInitializerService
    {
        private readonly AppDbContext _context;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Setting> _settingRepository;
        private readonly IRepository<AppIdentityPermission> _permissions;
        private readonly IRepository<AppRolePermission> _rolePermission;
        private readonly ILogger Log = Serilog.Log.ForContext<DbInitializerService>();

        public DbInitializerService(AppDbContext context, RoleManager<AppRole> roleManager, UserManager<User> userManager, IUnitOfWork unitOfWork)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _settingRepository = unitOfWork.Repository<Setting>();
            _permissions = unitOfWork.Repository<AppIdentityPermission>();
            _rolePermission = unitOfWork.Repository<AppRolePermission>();
        }

        public void Initialize()
        {
            //create database schema if none exists
            _context.Database.Migrate();
            _context.Database.EnsureCreated();

            // Check roles
            var appRoles = Enum.GetValues(typeof(Role));
            foreach (var role in appRoles)
            {
                var isRoleExist = _roleManager.RoleExistsAsync(role.ToString()).Result;
                if (!isRoleExist)
                {
                    var identity = _roleManager.CreateAsync(new AppRole(role.ToString())).Result;
                }
            }
            CreatePermissionsInRole();

            //Check settings
            var dbSettings = _settingRepository.GetAllQueryable();
            var requiredAppSettings = RequiredAppSettings();
            foreach (var appSetting in requiredAppSettings)
            {
                var relatedDbSetting = dbSettings.FirstOrDefault(s => s.ParamName == appSetting.Key);
                if (relatedDbSetting == null)
                {
                    _settingRepository.Insert(new Setting { ParamName = appSetting.Key, ParamValue = appSetting.Value });
                }
            }
            _unitOfWork.SaveChanges();

            CreateUser(new NewAdminModel
            {
                Email = "web_admin@gmail.com",
                FirstName = "Web",
                LastName = "Admin",
                Password = "admin2019"
            }, Role.Admin);

        }

        private Dictionary<string, string> RequiredAppSettings()
        {
            var settings = new Dictionary<string, string>
            {
                {"FacebookAppId", ""},
                {"FacebookSecret",""},
                {"AmazonBucketName","" },
                {"AmazonAccessKeyId","" },
                {"AmazonSecretAccessKey","" },
                {"AmazonsS3Region","us-east-1" },
                {"AmazonCDNUrl", ""},
                {"AppStoreLink", ""},
                {"Region", "us-east-1"},
                {"AppEmail", ""},
                {"Charset", "UTF-8"},
                {"AmazonIoSEndpointArn", ""},
                {"ResendNotificationsMaxTryCount", "5"},
                {"InviteLink", "userid:{0}"},
                {"ForgotPasswordWebLink", ""},
                {"ForgotPasswordDeltaExpDay", "1"}
            };
            return settings;
        }

        private void CreateUser(NewAdminModel model, Role role)
        {
            try
            {
                if (_userManager.Users.Count(x => x.Email == model.Email) > 0)
                {
                    return;
                }

                var user = Mapper.Map<User>(model);
                var result = _userManager.CreateAsync(user, model.Password).Result;

                if (!result.Succeeded)
                {
                    var errors = (List<IdentityError>)result.Errors;
                    Log.Error(string.Join("; ", Array.ConvertAll(errors.ToArray(), i => i.Description.ToString())));
                }

                _userManager.AddToRoleAsync(user, role.ToString()).Wait();
            }
            catch (Exception e)
            {
                Log.Error(e, "An {@Exception} has been occurred in CreateAdminAsync");
            }

        }

        private void CreatePermissionsInRole()
        {
            var appPermission = Enum.GetValues(typeof(Permission));
            foreach (var permisson in appPermission)
            {
                Permission value = (Permission)permisson;
                var permissionName = value.ToString();
                if (!_permissions.Any(p => p.Name == permissionName))
                {
                    var permissionItem = new AppIdentityPermission() { Id = (int)permisson, Name = permissionName, Description = permissionName };
                    _permissions.Insert(permissionItem);
                    string[] descriptions = value.GetEnumDescription().Split(',');
                    var roles = _roleManager.Roles.Where(r => descriptions.Contains(r.Name)).ToList();
                    foreach (var role in roles)
                    {
                        _rolePermission.Insert(new AppRolePermission() { PermissionId = permissionItem.Id, RoleId = role.Id });
                    }
                }
            }
            _unitOfWork.SaveChanges();
        }
    }
}

