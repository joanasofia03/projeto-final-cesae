public interface INotificationService
{
    Task<Fact_Notifications> CreateNotificationAsync(
        int? appUserId,
        DateTime notificationDate,
        string notificationType,
        string channel,
        string status);
    Task<Fact_Notifications?> GetNotificationByIdAsync(int id);
    Task<List<Fact_Notifications>> GetNotificationsByUserIdAsync(int appUserId);
}
