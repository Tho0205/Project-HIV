using System;
using System.ComponentModel.DataAnnotations;

namespace HIV.DTOs
{
    public class ExaminationResultDto
    {
        public int ExamId { get; set; }
        public string? DoctorName { get; set; }
        public string? Result { get; set; }
        public int? Cd4Count { get; set; }
        public int? HivLoad { get; set; }
        public DateOnly? ExamDate { get; set; }
        public string? CustomizedArvProtocolDetails { get; set; }
    }

    public class CreateExaminationDto
    {
        [Required(ErrorMessage = "PatientId là bắt buộc")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "DoctorId là bắt buộc")]
        public int DoctorId { get; set; }

        public string? CurrentCondition { get; set; } 

        [Required(ErrorMessage = "Kết quả xét nghiệm là bắt buộc")]
        public string? Result { get; set; }

        public string? Cd4Range { get; set; } 
        public string? HivLoadRange { get; set; } 

        [Required(ErrorMessage = "Ngày xét nghiệm là bắt buộc")]
        public DateOnly ExamDate { get; set; }

        // COMPUTED PROPERTIES
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

    public class UpdateExaminationDto
    {
        public string? Result { get; set; }
        public int? Cd4Count { get; set; }
        public int? HivLoad { get; set; }
    }
}