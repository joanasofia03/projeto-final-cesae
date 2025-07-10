using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Fact_Notifications
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set; }

    public int? AppUser_ID { get; set; }
    public AppUser? AppUser { get; set; }

    public int? Time_ID { get; set; }
    public Dim_Time? Time { get; set; }

    [StringLength(50, ErrorMessage = "Notification type cannot exceed 50 characters.")]
    [RegularExpression(@"^[A-Za-z\s_-]*$", ErrorMessage = "Notification type must contain only letters, spaces, underscores or hyphens.")]
    public string? Notification_Type { get; set; }

    [StringLength(20, ErrorMessage = "Channel cannot exceed 20 characters.")]
    [RegularExpression(@"^[A-Za-z\s_-]*$", ErrorMessage = "Channel must contain only letters, spaces, underscores or hyphens.")]
    public string? Channel { get; set; }

    [StringLength(20, ErrorMessage = "Status cannot exceed 20 characters.")]
    [RegularExpression(@"^[A-Za-z\s_-]*$", ErrorMessage = "Status must contain only letters, spaces, underscores or hyphens.")]
    public string? Fact_Notifications_Status { get; set; }
}
