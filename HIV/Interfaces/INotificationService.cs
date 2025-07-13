using HIV.DTOs;

namespace HIV.Interfaces
{
    public interface INotificationService
    {
        Task<List<NotificationDto>> GetNotificationsForUser(int userId);
        Task<NotificationDto> CreateNotification(CreateNotificationDto dto);
        Task<bool> MarkAsRead(int notificationId);
        Task<bool> MarkAllAsRead(int userId);
        Task<bool> DeleteNotification(int notificationId);
        Task<List<NotificationDto>> GetFilteredNotifications(NotificationFilterDto filter);

        // Các hàm tự động tạo thông báo
        Task CreateMedicationReminders(int patientId);
        Task CreateAppointmentReminders(int appointmentId);
    }
}