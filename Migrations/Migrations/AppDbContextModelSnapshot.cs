﻿// <auto-generated />
using System;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Migrations.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.1-servicing-10028")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Core.Entities.ActivationCode", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code")
                        .HasMaxLength(10);

                    b.Property<DateTime>("CreateDate");

                    b.Property<string>("EntityActivation")
                        .HasMaxLength(255);

                    b.Property<DateTime?>("ExpireDate");

                    b.Property<bool>("IsActivated");

                    b.HasKey("Id");

                    b.ToTable("ActivationCodes");
                });

            modelBuilder.Entity("Core.Entities.EFMigrationsHistory", b =>
                {
                    b.Property<string>("MigrationId")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(255)
                        .HasDefaultValue("False");

                    b.Property<string>("ProductVersion");

                    b.HasKey("MigrationId");

                    b.ToTable("EFMigrationsHistory");
                });

            modelBuilder.Entity("Core.Entities.Log", b =>
                {
                    b.Property<long>("Id");

                    b.Property<double>("ExecutionTime");

                    b.Property<string>("ErrorMessage");

                    b.Property<string>("ErrorSource")
                        .HasMaxLength(512);

                    b.Property<DateTime?>("EventDateTime")
                        .HasColumnType("datetime");

                    b.Property<string>("InnerErrorMessage");

                    b.Property<string>("InputParameters");

                    b.Property<string>("Path")
                        .HasMaxLength(512);

                    b.Property<string>("RequestType")
                        .HasMaxLength(512);

                    b.Property<string>("Response");

                    b.Property<string>("StackTrace");

                    b.Property<string>("Type")
                        .HasMaxLength(512);

                    b.Property<long?>("UserId");

                    b.HasKey("Id", "ExecutionTime");

                    b.ToTable("Log");
                });

            modelBuilder.Entity("Core.Entities.MessageHistory", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ActivityType");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime");

                    b.Property<string>("ErrorDescription");

                    b.Property<string>("EventCode");

                    b.Property<string>("FormattedMessage");

                    b.Property<bool>("IsRead");

                    b.Property<string>("Message");

                    b.Property<string>("MessageId");

                    b.Property<string>("Payload");

                    b.Property<int>("Status");

                    b.Property<string>("Title");

                    b.Property<short>("TryCount");

                    b.Property<int>("Type");

                    b.Property<DateTime>("UpdateDate")
                        .HasColumnType("datetime");

                    b.Property<long?>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("MessagesHistory");
                });

            modelBuilder.Entity("Core.Entities.Notification.AmazonTopic", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Arn");

                    b.Property<DateTime>("CreateDate");

                    b.Property<string>("DeviceToken");

                    b.Property<DateTime>("UpdateDate");

                    b.HasKey("Id");

                    b.ToTable("AmazonTopics");
                });

            modelBuilder.Entity("Core.Entities.QueueMessage", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("AttemptsCount");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime");

                    b.Property<string>("ExceptionMessage")
                        .HasMaxLength(512);

                    b.Property<short>("MessageState");

                    b.Property<string>("MessageType");

                    b.Property<byte[]>("SerializedMessage");

                    b.Property<DateTime>("UpdateDate")
                        .HasColumnType("datetime");

                    b.Property<long>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("QueueMessage");
                });

            modelBuilder.Entity("Core.Entities.Setting", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ParamName")
                        .HasMaxLength(512);

                    b.Property<string>("ParamValue");

                    b.HasKey("Id");

                    b.ToTable("Setting");
                });

            modelBuilder.Entity("Core.Entities.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("EnableNotifications");

                    b.Property<bool>("EnablePush");

                    b.Property<string>("FbToken")
                        .HasMaxLength(256);

                    b.Property<string>("FirstName")
                        .HasMaxLength(40);

                    b.Property<string>("Image");

                    b.Property<bool>("IsActive");

                    b.Property<bool>("IsAdmin");

                    b.Property<bool>("IsDelete");

                    b.Property<string>("LastName")
                        .HasMaxLength(40);

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<long>("UnreadMessageCount");

                    b.Property<DateTime>("UpdateDate")
                        .HasColumnType("datetime");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.ToTable("User");
                });

            modelBuilder.Entity("Core.Entities.UserDevice", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreateDate");

                    b.Property<string>("DeviceId");

                    b.Property<string>("DevicePushToken");

                    b.Property<bool>("IsEnabledPush");

                    b.Property<short>("OsType");

                    b.Property<int>("Status");

                    b.Property<DateTime>("UpdateDate");

                    b.HasKey("Id");

                    b.ToTable("UserDevice");
                });

            modelBuilder.Entity("Core.Entities.UserForgotPassword", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("ExpireDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Guid")
                        .HasMaxLength(36);

                    b.Property<short>("Status");

                    b.Property<long>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserForgotPasswords");
                });

            modelBuilder.Entity("Core.Entities.UserToken", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AuthToken");

                    b.Property<long?>("UserDeviceId");

                    b.Property<long>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserDeviceId");

                    b.HasIndex("UserId");

                    b.ToTable("UserToken");
                });

            modelBuilder.Entity("Core.IdentityEntities.AppIdentityPermission", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("AppIdentityPermissions");
                });

            modelBuilder.Entity("Core.IdentityEntities.AppRole", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.ToTable("AppRole");
                });

            modelBuilder.Entity("Core.IdentityEntities.AppRolePermission", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("PermissionId");

                    b.Property<long>("RoleId");

                    b.HasKey("Id");

                    b.HasIndex("PermissionId");

                    b.HasIndex("RoleId");

                    b.ToTable("AppRolePermissions");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<long>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<long>("RoleId");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AppRoleClaim");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<long>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<long>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AppUserClaim");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<long>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(255);

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(255);

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<long>("UserId");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AppUserLogin");

                    b.HasDiscriminator<string>("Discriminator").HasValue("IdentityUserLogin<long>");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<long>", b =>
                {
                    b.Property<long>("UserId");

                    b.Property<long>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AppUserRole");
                });

            modelBuilder.Entity("Core.IdentityEntities.AppUserLogin", b =>
                {
                    b.HasBaseType("Microsoft.AspNetCore.Identity.IdentityUserLogin<long>");

                    b.ToTable("AppUserLogin");

                    b.HasDiscriminator().HasValue("AppUserLogin");
                });

            modelBuilder.Entity("Core.Entities.MessageHistory", b =>
                {
                    b.HasOne("Core.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Core.Entities.QueueMessage", b =>
                {
                    b.HasOne("Core.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Core.Entities.UserForgotPassword", b =>
                {
                    b.HasOne("Core.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Core.Entities.UserToken", b =>
                {
                    b.HasOne("Core.Entities.UserDevice", "UserDevice")
                        .WithMany("UserTokens")
                        .HasForeignKey("UserDeviceId");

                    b.HasOne("Core.Entities.User", "User")
                        .WithMany("UserTokens")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Core.IdentityEntities.AppRolePermission", b =>
                {
                    b.HasOne("Core.IdentityEntities.AppIdentityPermission", "Permission")
                        .WithMany("Roles")
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Core.IdentityEntities.AppRole", "Role")
                        .WithMany("Permissions")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<long>", b =>
                {
                    b.HasOne("Core.IdentityEntities.AppRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<long>", b =>
                {
                    b.HasOne("Core.Entities.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<long>", b =>
                {
                    b.HasOne("Core.Entities.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<long>", b =>
                {
                    b.HasOne("Core.IdentityEntities.AppRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Core.Entities.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
