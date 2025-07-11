using Microsoft.EntityFrameworkCore;

public class NotificationService : INotificationService
{
    private readonly AnalyticPlatformContext _context;

    public NotificationService(AnalyticPlatformContext context)
    {
        _context = context;
    }

    public async Task<Fact_Notifications> CreateNotificationAsync(int? appUserId, DateTime notificationDate, string notificationType, string channel, string status)
    {
        var dimTime = new Dim_Time
        {
            date_Date = notificationDate,
            date_Year = notificationDate.Year,
            date_Month = notificationDate.Month,
            date_Quarter = (notificationDate.Month - 1) / 3 + 1,
            Weekday_Name = notificationDate.DayOfWeek.ToString(),
            Is_Weekend = notificationDate.DayOfWeek == DayOfWeek.Saturday || notificationDate.DayOfWeek == DayOfWeek.Sunday
        };

        _context.Dim_Time.Add(dimTime);
        await _context.SaveChangesAsync();

        var notification = new Fact_Notifications
        {
            AppUser_ID = appUserId,
            Time_ID = dimTime.ID,
            Notification_Type = notificationType,
            Channel = channel,
            Fact_Notifications_Status = status
        };

        _context.Fact_Notifications.Add(notification);
        await _context.SaveChangesAsync();

        return notification;
    }

    public async Task<Fact_Notifications?> GetNotificationByIdAsync(int id)
    {
        return await _context.Fact_Notifications
            .Include(n => n.Time)
            .Include(n => n.AppUser)
            .FirstOrDefaultAsync(n => n.ID == id);
    }
}
