using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class Fact_NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public Fact_NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    // POST: http://localhost:5146/api/fact_notifications/create
    [HttpPost("create")]
    [HttpPost]
    public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        int? userId = userIdClaim != null && int.TryParse(userIdClaim.Value, out var id) ? id : (int?)null;

        var notificationDate = dto.NotificationDate ?? DateTime.UtcNow;

        var notification = await _notificationService.CreateNotificationAsync(
            userId,
            notificationDate,
            dto.NotificationType,
            dto.Channel,
            dto.Status);

        if (notification == null)
            return StatusCode(500, "Error creating notification.");

        var result = new ReadNotificationDto
        {
            UserName = notification.AppUser?.FullName,
            NotificationType = notification.Notification_Type,
            Channel = notification.Channel,
            Status = notification.Fact_Notifications_Status,
            NotificationDate = notification.Time?.date_Date
        };

        return CreatedAtAction(nameof(GetNotification), new { id = notification.ID }, result);
    }

    // GET: http://localhost:5146/api/fact_notifications/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetNotification(int id)
    {
        var notification = await _notificationService.GetNotificationByIdAsync(id);
        if (notification == null)
            return NotFound();

        var result = new ReadNotificationDto
        {
            UserName = notification.AppUser?.FullName,
            NotificationType = notification.Notification_Type,
            Channel = notification.Channel,
            Status = notification.Fact_Notifications_Status,
            NotificationDate = notification.Time?.date_Date
        };

        return Ok(result);
    }
    
    // GET: http://localhost:5146/api/fact_notifications/my-notifications
    [HttpGet("my-notifications")]
    public async Task<IActionResult> GetMyNotifications()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized();

        var notifications = await _notificationService.GetNotificationsByUserIdAsync(userId);

        var result = notifications.Select(notification => new ReadNotificationDto
        {
            UserName = notification.AppUser?.FullName,
            NotificationType = notification.Notification_Type,
            Channel = notification.Channel,
            Status = notification.Fact_Notifications_Status,
            NotificationDate = notification.Time?.date_Date
        }).ToList();

        return Ok(result);
    }

}
