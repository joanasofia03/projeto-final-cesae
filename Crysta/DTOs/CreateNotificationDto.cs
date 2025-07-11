using System.ComponentModel.DataAnnotations;

public class CreateNotificationDto
{
    public DateTime? NotificationDate { get; set; }

    [StringLength(50, ErrorMessage = "Notification type cannot exceed 50 characters.")]
    [RegularExpression(@"^[A-Za-z\s_-]*$", ErrorMessage = "Notification type must contain only letters, spaces, underscores or hyphens.")]
    public string NotificationType { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "Channel cannot exceed 20 characters.")]
    [RegularExpression(@"^[A-Za-z\s_-]*$", ErrorMessage = "Channel must contain only letters, spaces, underscores or hyphens.")]
    public string Channel { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "Status cannot exceed 20 characters.")]
    [RegularExpression(@"^[A-Za-z\s_-]*$", ErrorMessage = "Status must contain only letters, spaces, underscores or hyphens.")]
    public string Status { get; set; } = string.Empty;
}
