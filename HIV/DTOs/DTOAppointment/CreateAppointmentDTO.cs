namespace HIV.DTOs.DTOAppointment
{
    public class CreateAppointmentDTO
    {
        public int PatientId { get; set; }

        public int ScheduleId { get; set; }

        public int doctorId { get; set; }

        public string? Note { get; set; }

        public bool? IsAnonymous { get; set; } = false;

        public DateTime AppointmentDate { get; set; }

    }
}
