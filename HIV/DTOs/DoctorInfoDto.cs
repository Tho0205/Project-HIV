using System.ComponentModel.DataAnnotations;

namespace HIV.DTOs
{
    public class DoctorInfoDto
    {
        public int DoctorId { get; set; }
        public string? Degree { get; set; }
        public string? Specialization { get; set; }
        public int? ExperienceYears { get; set; }
        public string? DoctorAvatar { get; set; }
        public string Status { get; set; } = "ACTIVE";

        public string? DoctorName { get; set; } // Optional: show related User.Name
    }
    public class CreateDoctorInfoDto
    {
        [Required]
        public int DoctorId { get; set; }

        public string? Degree { get; set; }
        public string? Specialization { get; set; }
        public int? ExperienceYears { get; set; }
        public string? DoctorAvatar { get; set; }
    }
    public class UpdateDoctorInfoDto
    {
        public string? Degree { get; set; }
        public string? Specialization { get; set; }
        public int? ExperienceYears { get; set; }
        public string? DoctorAvatar { get; set; }
        public string Status { get; set; } = "ACTIVE";
    }
}
