using HIV.Models;

namespace HIV.DTOs
{
    public class ScheduleDTO
    {
        public int ScheduleId { get; set; }

        public int? DoctorId { get; set; }

        public DateTime? ScheduledTime { get; set; }

        public string? Room { get; set; }

        public string? Status { get; set; } // Thêm Status

        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

        public virtual User? Doctor { get; set; }
    }
}
