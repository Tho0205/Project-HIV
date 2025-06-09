using System;

namespace HIV.DTOs
{
    public class UserWithAppointmentDto
    {
        public int UserId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public string? ConsultationType { get; set; }
        public string? Gender { get; set; }
        public string? DoctorName { get; set; }
        public int AppointmentId { get; set; }
        public string? Role { get; set; }
    }

    public class UserDetailDto
    {
        public int UserId { get; set; }
        public string? FullName { get; set; }
        public DateOnly? Birthdate { get; set; }
        public string? Gender { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? UserAvatar { get; set; }
        public string? Role { get; set; }
        public int Age => Birthdate.HasValue ? DateTime.Now.Year - Birthdate.Value.Year : 0;
    }
}