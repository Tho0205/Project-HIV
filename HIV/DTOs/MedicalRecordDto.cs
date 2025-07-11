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

    // DTO chứa thông tin chi tiết của Medical Record
    public class MedicalRecordDetailDto
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

        // Thông tin Examination liên quan
        public ExaminationDetailDto? Examination { get; set; }

        // Thông tin ARV Protocol liên quan
        public CustomizedArvProtocolDto? CustomizedProtocol { get; set; }
    }

    // DTO cho thông tin Examination
    public class ExaminationDetailDto
    {
        public int ExamId { get; set; }
        public string? Result { get; set; }
        public int? Cd4Count { get; set; }
        public int? HivLoad { get; set; }
        public DateOnly? ExamDate { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // DTO cho Customized ARV Protocol
    public class CustomizedArvProtocolDto
    {
        public int CustomProtocolId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; }

        // Thông tin Base Protocol nếu có
        public string? BaseProtocolName { get; set; }

        // Danh sách các ARV trong protocol
        public List<ArvDetailInProtocolDto> ArvDetails { get; set; } = new();
    }

    // DTO cho thông tin ARV trong protocol
    public class ArvDetailInProtocolDto
    {
        public int ArvId { get; set; }
        public string? ArvName { get; set; }
        public string? ArvDescription { get; set; }
        public string? Dosage { get; set; }
        public string? UsageInstruction { get; set; }
        public string Status { get; set; }
    }
}