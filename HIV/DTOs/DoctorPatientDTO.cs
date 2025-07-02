using System.ComponentModel.DataAnnotations;

namespace HIV.DTOs.DoctorPatient
{
    public class DoctorPatientListDto
    {
        public int AccountId { get; set; }
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Gender { get; set; } = "Other";
        public DateOnly? Birthdate { get; set; }
        public string Address { get; set; } = string.Empty;
        public string? UserAvatar { get; set; }
        public string status { get; set; } = "ACTIVE";
        public int AppointmentCount { get; set; }
        public DateTime? LastAppointmentDate { get; set; }
    }

    public class DoctorPatientUpdateDto
    {
        [Required]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "Tên không được quá 100 ký tự")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public string Gender { get; set; } = "Other";

        public DateOnly? Birthdate { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "Địa chỉ không được quá 200 ký tự")]
        public string Address { get; set; } = string.Empty;

        [Required]
        public string Status { get; set; } = "ACTIVE";
    }

    public class DoctorPatientStatsDto
    {
        public int TotalPatients { get; set; }
        public int ActivePatients { get; set; }
        public int InactivePatients { get; set; }
        public int RecentAppointments { get; set; }
        public int PendingAppointments { get; set; }
    }

    public class DoctorPatientsResponseDto
    {
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public List<DoctorPatientListDto> Data { get; set; } = new();
        public DoctorPatientStatsDto Stats { get; set; } = new();
    }

    public class PatientHistoryDto
    {
        public List<AppointmentHistoryDto> Appointments { get; set; } = new();
        public List<ExaminationHistoryDto> Examinations { get; set; } = new();
    }

    public class AppointmentHistoryDto
    {
        public int AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Note { get; set; }
        public string? Room { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ExaminationHistoryDto
    {
        public int ExamId { get; set; }
        public DateOnly? ExamDate { get; set; }
        public string Result { get; set; } = string.Empty;
        public int? Cd4Count { get; set; }
        public int? HivLoad { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}