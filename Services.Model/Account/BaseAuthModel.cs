using System.ComponentModel.DataAnnotations;
using Core.Enums;

namespace Services.Model.Account
{
    public class BaseAuthModel
    {
        /// <summary>
        /// Gets or sets the device identifier.
        /// </summary>
        /// <value>
        /// The device identifier.
        /// </value>
        [Required]
        public string DeviceId { get; set; }

        /// <summary>
        /// Type of the push notification
        /// </summary>
        [Required]
        public OsType OsType { get; set; }

        public string DeviceToken { get; set; }
    }
}
