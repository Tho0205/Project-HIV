using HIV.DTOs;
using HIV.Interfaces;
using HIV.Models;
using Microsoft.EntityFrameworkCore;

namespace HIV.Repository
{
    public class NotificationService : INotificationService
    {
        private readonly AppDbContext _context;

        public NotificationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<NotificationDto>> GetNotificationsForUser(int userId)
        {
            var notifications = await _context.Notification
                .Where(n => n.UserId == userId && n.Status == "ACTIVE")
                .OrderByDescending(n => n.ScheduledTime)
                .ToListAsync();

            return notifications.Select(n => new NotificationDto
            {
                NotificationId = n.NotificationId,
                Type = n.Type,
                Message = n.Message,
                ScheduledTime = n.ScheduledTime,
                IsRead = n.IsRead,
                TimeAgo = GetTimeAgo(n.ScheduledTime),
                RelatedId = GetRelatedId(n)
            }).ToList();
        }

        public async Task<NotificationDto> CreateNotification(CreateNotificationDto dto)
        {
            var notification = new Notification
            {
                UserId = dto.UserId,
                Type = dto.Type,
                Message = dto.Message,
                ScheduledTime = dto.ScheduledTime,
                IsRead = false,
                CreatedAt = DateTime.Now,
                Status = "ACTIVE",
                AppointmentId = dto.AppointmentId,
                ProtocolId = dto.ProtocolId,
                ExaminationId = dto.ExaminationId
            };

            _context.Notification.Add(notification);
            await _context.SaveChangesAsync();

            return new NotificationDto
            {
                NotificationId = notification.NotificationId,
                Type = notification.Type,
                Message = notification.Message,
                ScheduledTime = notification.ScheduledTime,
                IsRead = notification.IsRead,
                TimeAgo = GetTimeAgo(notification.ScheduledTime),
                RelatedId = GetRelatedId(notification)
            };
        }

        public async Task<bool> MarkAsRead(int notificationId)
        {
            var notification = await _context.Notification.FindAsync(notificationId);
            if (notification == null) return false;

            notification.IsRead = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkAllAsRead(int userId)
        {
            var notifications = await _context.Notification
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteNotification(int notificationId)
        {
            var notification = await _context.Notification.FindAsync(notificationId);
            if (notification == null) return false;

            notification.Status = "CANCELLED";
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<NotificationDto>> GetFilteredNotifications(NotificationFilterDto filter)
        {
            var query = _context.Notification.AsQueryable();

            if (filter.UserId.HasValue)
                query = query.Where(n => n.UserId == filter.UserId);

            if (!string.IsNullOrEmpty(filter.Type))
                query = query.Where(n => n.Type == filter.Type);

            if (filter.IsRead.HasValue)
                query = query.Where(n => n.IsRead == filter.IsRead);

            if (filter.FromDate.HasValue)
                query = query.Where(n => n.ScheduledTime >= filter.FromDate);

            if (filter.ToDate.HasValue)
                query = query.Where(n => n.ScheduledTime <= filter.ToDate);

            var notifications = await query
                .Where(n => n.Status == "ACTIVE")
                .OrderByDescending(n => n.ScheduledTime)
                .ToListAsync();

            return notifications.Select(n => new NotificationDto
            {
                NotificationId = n.NotificationId,
                Type = n.Type,
                Message = n.Message,
                ScheduledTime = n.ScheduledTime,
                IsRead = n.IsRead,
                TimeAgo = GetTimeAgo(n.ScheduledTime),
                RelatedId = GetRelatedId(n)
            }).ToList();
        }

        public async Task CreateMedicationReminders(int patientId)
        {
            var protocol = await _context.CustomizedARVProtocols
                .Include(p => p.Details)
                    .ThenInclude(d => d.Arv)
                .Where(p => p.PatientId == patientId && p.Status == "ACTIVE")
                .OrderByDescending(p => p.CustomProtocolId)
                .FirstOrDefaultAsync();

            if (protocol == null) return;

            // Huỷ nhắc nhở cũ
            var oldReminders = await _context.Notification
                .Where(n => n.UserId == patientId && n.Type == "medication" && n.Status == "ACTIVE")
                .ToListAsync();

            foreach (var reminder in oldReminders)
            {
                reminder.Status = "COMPLETED";
            }

            // Nhóm thuốc theo hướng dẫn sử dụng
            var groupedByUsage = protocol.Details
                .Where(d => d.Status == "ACTIVE")
                .GroupBy(d => d.UsageInstruction?.Trim() ?? "Khác")
                .ToList();

            for (int i = 0; i < 7; i++)
            {
                var currentDate = DateTime.Today.AddDays(i);

                foreach (var group in groupedByUsage)
                {
                    var usageTime = group.Key;
                    var hour = usageTime switch
                    {
                        string s when s.Contains("sáng", StringComparison.OrdinalIgnoreCase) => 6,
                        string s when s.Contains("tối", StringComparison.OrdinalIgnoreCase) => 18,
                        string s when s.Contains("trước ăn", StringComparison.OrdinalIgnoreCase) => 7,
                        string s when s.Contains("trước ngủ", StringComparison.OrdinalIgnoreCase) => 21,
                        _ => 20
                    };

                    var reminderTime = currentDate.AddHours(hour);

                    var messageLines = group.Select(d =>
                        $"- {d.Arv?.Name} {d.Dosage}"
                    );
                    var fullMessage = $"💊 Nhắc bạn uống thuốc {usageTime}:\n" + string.Join("\n", messageLines);

                    await CreateNotification(new CreateNotificationDto
                    {
                        UserId = patientId,
                        Type = "medication",
                        Message = fullMessage,
                        ScheduledTime = reminderTime,
                        ProtocolId = protocol.CustomProtocolId
                    });
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task CreateAppointmentReminders(int appointmentId)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Schedule)
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);

            if (appointment == null || appointment.PatientId == null) return;

            // Xóa thông báo cũ liên quan lịch hẹn này
            var oldReminders = await _context.Notification
                .Where(n => n.AppointmentId == appointmentId && n.Status == "ACTIVE")
                .ToListAsync();

            foreach (var reminder in oldReminders)
            {
                reminder.Status = "COMPLETED";
            }

            var doctorName = appointment.Doctor?.FullName ?? "Bác sĩ chưa xác định";
            var room = appointment.Schedule?.Room ?? "Phòng chưa rõ";
            var appointmentTime = appointment.AppointmentDate;

            string messageBase = $"Lịch hẹn với bác sĩ {doctorName} vào {appointmentTime:HH:mm dd/MM/yyyy} tại phòng {room}.";

            // 🟢 Gửi thông báo NGAY khi xác nhận
            await CreateNotification(new CreateNotificationDto
            {
                UserId = appointment.PatientId,
                Type = "appointment",
                Message = $"📅 {messageBase}",
                ScheduledTime = DateTime.Now,
                AppointmentId = appointmentId
            });

            // 🟡 Gửi trước 12 tiếng
            var reminderTime = appointmentTime.AddHours(-12);
            if (reminderTime > DateTime.Now)
            {
                await CreateNotification(new CreateNotificationDto
                {
                    UserId = appointment.PatientId,
                    Type = "appointment",
                    Message = $"🔔 Nhắc lịch: {messageBase}",
                    ScheduledTime = reminderTime,
                    AppointmentId = appointmentId
                });
            }

            await _context.SaveChangesAsync();
        }

        // Helper methods
        private string GetTimeAgo(DateTime dateTime)
        {
            var timeSpan = DateTime.Now - dateTime;
            
            if (timeSpan.TotalMinutes < 1)
                return "Vừa xong";
            
            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes} phút trước";
            
            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours} giờ trước";
            
            return $"{(int)timeSpan.TotalDays} ngày trước";
        }

        private int? GetRelatedId(Notification notification)
        {
            return notification.Type switch
            {
                "medication" => notification.ProtocolId,
                "appointment" => notification.AppointmentId,
                "examination" => notification.ExaminationId,
                _ => null
            };
        }
    }
}