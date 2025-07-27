namespace HIV.DTOs
{
    public class NotificationDto
    {
        public int NotificationId { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
        public DateTime ScheduledTime { get; set; }
        public bool IsRead { get; set; }
        public string TimeAgo { get; set; } 
        public int? RelatedId { get; set; } 
    }

    public class CreateNotificationDto
    {
        public int UserId { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
        public DateTime ScheduledTime { get; set; }
        public int? AppointmentId { get; set; }
        public int? ProtocolId { get; set; }
        public int? ExaminationId { get; set; }
    }

    public class NotificationFilterDto
    {
        public int? UserId { get; set; }
        public string? Type { get; set; }
        public bool? IsRead { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}