using System.ComponentModel.DataAnnotations;

namespace HIV.DTOs
{
    /// <summary>
    /// DTO để tạo kết quả xét nghiệm HIV mới
    /// </summary>
    public class CreateHIVExaminationDTO
    {
        [Required(ErrorMessage = "Vui lòng chọn bệnh nhân")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn bác sĩ")]
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày xét nghiệm")]
        public DateOnly ExamDate { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập kết quả xét nghiệm")]
        [StringLength(1000, ErrorMessage = "Kết quả không được vượt quá 1000 ký tự")]
        public string Result { get; set; } = "";

        public string? Cd4Range { get; set; } // "> 200", "100-200", "< 100"
        public string? HivLoadRange { get; set; } // "Không Phát Hiện", "< 50", v.v.

        [StringLength(2000, ErrorMessage = "Tình trạng hiện tại không được vượt quá 2000 ký tự")]
        public string? CurrentCondition { get; set; }

        // Computed properties để convert range thành số
        public int? Cd4Count
        {
            get
            {
                return Cd4Range switch
                {
                    "> 200" => 250,
                    "100-200" => 150,
                    "< 100" => 50,
                    _ => null
                };
            }
        }

        public int? HivLoad
        {
            get
            {
                return HivLoadRange switch
                {
                    "Không Phát Hiện" => 0,
                    "< 50 copies/ml" => 25,
                    "50-1000 copies/ml" => 500,
                    "> 1000 copies/ml" => 1500,
                    _ => null
                };
            }
        }
    }

    /// <summary>
    /// DTO để cập nhật kết quả xét nghiệm
    /// </summary>
    public class UpdateHIVExaminationDTO
    {
        [StringLength(1000, ErrorMessage = "Kết quả không được vượt quá 1000 ký tự")]
        public string? Result { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "CD4 Count phải >= 0")]
        public int? Cd4Count { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "HIV Load phải >= 0")]
        public int? HivLoad { get; set; }

        [StringLength(2000, ErrorMessage = "Tóm tắt không được vượt quá 2000 ký tự")]
        public string? Summary { get; set; }
    }

    /// <summary>
    /// DTO trả về thông tin chi tiết kết quả xét nghiệm
    /// </summary>
    public class HIVExaminationResponseDTO
    {
        public int ExamId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = "";
        public string PatientEmail { get; set; } = "";
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = "";
        public DateOnly ExamDate { get; set; }
        public string Result { get; set; } = "";
        public int? Cd4Count { get; set; }
        public int? HivLoad { get; set; }
        public string Status { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public string? MedicalRecordSummary { get; set; }
    }

    /// <summary>
    /// DTO cho danh sách xét nghiệm (dạng tóm tắt)
    /// </summary>
    public class HIVExaminationSummaryDTO
    {
        public int ExamId { get; set; }
        public string PatientName { get; set; } = "";
        public string DoctorName { get; set; } = "";
        public DateOnly ExamDate { get; set; }
        public int? Cd4Count { get; set; }
        public int? HivLoad { get; set; }
        public string ResultSummary { get; set; } = "";
        public string Status { get; set; } = "";
    }

    /// <summary>
    /// DTO cho danh sách bác sĩ
    /// </summary>
    public class DoctorSimpleDTO
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = "";
        public string? Specialization { get; set; }
        public string? Degree { get; set; }
        public string DisplayName => !string.IsNullOrEmpty(Degree) ? $"{Degree} {FullName}" : FullName;
    }

    /// <summary>
    /// DTO cho response API chuẩn
    /// </summary>
    public class ApiResponseDTO<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }

        public static ApiResponseDTO<T> SuccessResult(T data, string message = "Thành công")
        {
            return new ApiResponseDTO<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static ApiResponseDTO<T> ErrorResult(string message, List<string>? errors = null)
        {
            return new ApiResponseDTO<T>
            {
                Success = false,
                Message = message,
                Errors = errors
            };
        }
    }
}