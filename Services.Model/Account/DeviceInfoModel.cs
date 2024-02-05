using System.ComponentModel.DataAnnotations;
using Core.Enums;

namespace Services.Model.Account
{
    public class DeviceInfoModel
    {
        /// <summary>
        /// Device Os Type
        /// </summary>
        [Required]
        public OsType OsType { get; set; }

        /// <summary>
        /// Device Push Token
        /// </summary>
        [Required]
        public string DeviceToken { get; set; }

        /// <summary>
        /// Device Id
        /// </summary>
        [Required]
        public string DeviceId { get; set; }
    }
}
