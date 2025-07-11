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

/* var notification = new Fact_Notifications
{
    AppUser_ID = 1,
    AppUser = new AppUser
    {
        // Assuming AppUser class has at least these properties
        ID = 1,
        UserName = "johndoe"
    },
    Time_ID = 20250711,
    Time = new Dim_Time
    {
        // Assuming Dim_Time class has at least these properties
        ID = 20250711,
        Date = new DateTime(2025, 7, 11)
    },
    Notification_Type = "Password Reset",
    Channel = "Email",
    Fact_Notifications_Status = "Sent"
}; */

