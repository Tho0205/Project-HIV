using System.ComponentModel.DataAnnotations;

namespace HIV.DTOs
{
    /// <summary>
    /// DTO chung cho cả Create và Update examination
    /// </summary>
    public class ExaminationFormDTO
    {
        public int? ExamId { get; set; } // Null for create, has value for update

        [Required(ErrorMessage = "Vui lòng chọn bệnh nhân")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn bác sĩ")]
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày xét nghiệm")]
        public DateOnly ExamDate { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập kết quả xét nghiệm")]
        [StringLength(1000)]
        public string Result { get; set; } = "";

        [Range(0, int.MaxValue)]
        public int? Cd4Count { get; set; }

        [Range(0, int.MaxValue)]
        public int? HivLoad { get; set; }
    }

    /// <summary>
    /// Simplified patient DTO
    /// </summary>
    public class PatientListDTO
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string? Phone { get; set; }
        public DateOnly? Birthdate { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public int ExamCount { get; set; }
        public DateOnly? LastExamDate { get; set; }
    }

    /// <summary>
    /// Simplified examination DTO
    /// </summary>
    public class ExaminationDTO
    {
        public int ExamId { get; set; }
        public string DoctorName { get; set; } = "";
        public DateOnly ExamDate { get; set; }
        public string Result { get; set; } = "";
        public int? Cd4Count { get; set; }
        public int? HivLoad { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// Base response DTO
    /// </summary>
    public class BaseResponseDTO<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public T? Data { get; set; }

        public static BaseResponseDTO<T> Ok(T data, string message = "Success")
            => new() { Success = true, Data = data, Message = message };

        public static BaseResponseDTO<T> Fail(string message)
            => new() { Success = false, Message = message };
    }
}