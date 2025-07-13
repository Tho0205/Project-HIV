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
        [StringLength(200, ErrorMessage = "Bằng cấp không được vượt quá 200 ký tự")]
        public string? Degree { get; set; }

        [StringLength(200, ErrorMessage = "Chuyên khoa không được vượt quá 200 ký tự")]
        public string? Specialization { get; set; }

        [Range(0, 50, ErrorMessage = "Số năm kinh nghiệm phải từ 0 đến 50")]
        public int? ExperienceYears { get; set; }

        public string? DoctorAvatar { get; set; }

        [RegularExpression("^(ACTIVE|INACTIVE|DELETED)$", ErrorMessage = "Trạng thái không hợp lệ")]
        public string? Status { get; set; } = "ACTIVE"; // Changed to nullable
    }
}