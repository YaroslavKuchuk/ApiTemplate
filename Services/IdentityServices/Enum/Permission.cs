
using System.ComponentModel;

namespace Services.IdentityServices.Enum
{
    public enum Permission
    {
        [Description("Admin")]
        BlockUser = 1,
        [Description("Admin")]
        UnBlockUser = 2,
        [Description("Admin")]
        ViewUserInfo = 3,
        [Description("Admin")]
        ViewAppUsers = 4,
        [Description("Admin")]
        CreateUser = 5,
        [Description("Admin")]
        DeleteUser = 6,
        [Description("Admin")]
        ViewAdmin = 7,
        [Description("Admin")]
        ViewBlockedAdmin = 8,
        [Description("Admin")]
        CreateAdmin = 9,
        [Description("Admin")]
        UpdateAdmin = 10,
        [Description("Admin")]
        BlockAdmin = 11,
        [Description("Admin")]
        UnBlockAdmin = 12,
        [Description("User,Admin")]
        HasUploadFile = 13,
        [Description("User,Admin")]
        GetCsvReport = 14

    }

    public static class PermissionString
    {
        /**Permissions of the admin***/

        public const string BlockUser = "BlockUser";
        public const string UnBlockUser = "UnBlockUser";
        public const string ViewUserInfo = "ViewUserInfo";
        public const string ViewAppUsers = "ViewAppUsers";
        public const string CreateUser = "CreateUser";
        public const string DeleteUser = "DeleteUser";
        public const string ViewAdmin = "ViewAdmin";
        public const string ViewBlockedAdmin = "ViewBlockedAdmin";
        public const string CreateAdmin = "CreateAdmin";
        public const string UpdateAdmin = "UpdateAdmin";
        public const string BlockAdmin = "BlockAdmin";
        public const string UnBlockAdmin = "UnBlockAdmin";

        /**User's permissions***/
        public const string HasUploadFile = "HasUploadFile";
        public const string GetCsvReport = "GetCsvReport";

    }
}
