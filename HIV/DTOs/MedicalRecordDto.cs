namespace HIV.DTOs
{
    public class MedicalRecordDto
    {
        public int RecordId { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int ExamId { get; set; }
        public int? CustomProtocolId { get; set; }
        public DateTime? ExamDate { get; set; }
        public TimeSpan? ExamTime { get; set; }
        public string Status { get; set; }
        public DateTime IssuedAt { get; set; }
        public string? Summary { get; set; }

        public string? DoctorName { get; set; }
        public string? PatientName { get; set; }
    }

    public class CreateMedicalRecordDto
    {
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int ExamId { get; set; }
        public int? CustomProtocolId { get; set; }
        public DateTime? ExamDate { get; set; }
        public TimeSpan? ExamTime { get; set; }
        public string? Summary { get; set; }
    }
    public class UpdateMedicalRecordDto
    {
        public DateTime? ExamDate { get; set; }
        public TimeSpan? ExamTime { get; set; }
        public string? Summary { get; set; }
        public string Status { get; set; }
    }
}
