namespace HIV.DTOs
{
    public class PatientOfDoctorDTO
    {
        public int PatientId { get; set; }
        public string FullName { get; set; }

        // Thông tin appointment
        public int AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
        public string AppoinmentType { get; set; }
        public bool IsAnonymous { get; set; }

        // Thông tin schedule
        public DateTime ScheduledTime { get; set; }
        public string Room { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
