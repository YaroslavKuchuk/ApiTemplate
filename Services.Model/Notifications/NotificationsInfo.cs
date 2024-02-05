namespace Services.Model.Notifications
{
    /// <summary>
    /// DTO for general notifications info
    /// </summary>
    public class NotificationsInfo
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public long UserId { get; set; }

        /// <summary>
        /// Gets or sets the new notifications count.
        /// </summary>
        /// <value>
        /// The new notifications count.
        /// </value>
        public int NewNotificationsCount { get; set; }
    }
}
