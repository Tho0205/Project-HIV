namespace HIV.Models
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public int? UserId { get; set; } // ID người dùng (bệnh nhân)
        public string Type { get; set; } // "medication", "appointment", "examination"
        public string Message { get; set; }
        public DateTime ScheduledTime { get; set; } // Thời gian nhắc nhở
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } // "ACTIVE", "COMPLETED", "CANCELLED"

        // Foreign keys
        public int? AppointmentId { get; set; }
        public int? ProtocolId { get; set; }
        public int? ExaminationId { get; set; }

        // Navigation properties
        public virtual User? User { get; set; }
        public virtual Appointment? Appointment { get; set; }
        public virtual CustomizedArvProtocol? Protocol { get; set; }
        public virtual Examination? Examination { get; set; }
    }
}