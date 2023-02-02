namespace Notl.MuseumMap.Admin.Common
{
    /// <summary>
    /// Defines the type of alert to be shown.
    /// </summary>
    public enum AlertType
    {
        /// <summary>
        /// Alert is just informational.
        /// </summary>
        Information,

        /// <summary>
        /// Alert is a warning.
        /// </summary>
        Warning,

        /// <summary>
        /// Alert is an error.
        /// </summary>
        Error,
    }

    /// <summary>
    /// Defines alert information passed to the alert dialog.
    /// </summary>
    public class AlertInfo
    {
        /// <summary>
        /// Alert type.
        /// </summary>
        public AlertType Type { get; set; }

        /// <summary>
        /// Title to appear on the page.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// The message to display.
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Additional details (perhaps error data) to display.
        /// </summary>
        public string? Details { get; set; }
    }
}
