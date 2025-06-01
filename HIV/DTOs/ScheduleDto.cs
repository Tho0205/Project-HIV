using System;

namespace HIV.DTOs
{
    public class ScheduleDto
    {
        public int ScheduleId { get; set; }
        public DateTime? ScheduledTime { get; set; }
        public string? Room { get; set; }
        public bool HasAppointment { get; set; }
        public string? PatientName { get; set; }
        public string? AppointmentNote { get; set; }
    }

    public class CreateScheduleDto
    {
        public int DoctorId { get; set; }
        public DateTime ScheduledTime { get; set; }
        public string? Room { get; set; }
    }

    public class UpdateScheduleDto
    {
        public DateTime? ScheduledTime { get; set; }
        public string? Room { get; set; }
    }
}