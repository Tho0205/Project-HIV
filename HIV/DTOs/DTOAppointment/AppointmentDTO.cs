using HIV.Models;

namespace HIV.DTOs.DTOAppointment
{
    public class AppointmentDTO
    {
        public int? AppointmentId { get; set; }

        public int PatientId { get; set; }

        public int ScheduleId { get; set; }

        public string? Note { get; set; }

        public bool? IsAnonymous { get; set; }
        
        public string? Status { get; set; }
        
        public int doctorId { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime AppointmentDate { get; set; }

    }
}
