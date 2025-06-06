using System;

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
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public string? Result { get; set; }
        public int? Cd4Count { get; set; }
        public int? HivLoad { get; set; }
        public DateOnly ExamDate { get; set; }
    }

    public class UpdateExaminationDto
    {
        public string? Result { get; set; }
        public int? Cd4Count { get; set; }
        public int? HivLoad { get; set; }
    }
}