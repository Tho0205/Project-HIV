namespace HIV.DTOs.DTOARVs
{
    public class PatientWithProtocolDto
    {
        public int PatientId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public LatestExaminationDto? LatestExamination { get; set; }
        public ProtocolInfoDto? CurrentProtocol { get; set; }
    }

    public class LatestExaminationDto
    {
        public DateOnly? ExamDate { get; set; }
        public int? Cd4Count { get; set; }
        public int? HivLoad { get; set; }
        public string? Result { get; set; }
    }

    public class ProtocolInfoDto
    {
        public int ProtocolId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsCustom { get; set; }
    }

    public class CustomProtocolDetailDto
    {
        public int DetailId { get; set; }
        public int ArvId { get; set; }
        public string ArvName { get; set; } = string.Empty;
        public string Dosage { get; set; } = string.Empty;
        public string? UsageInstruction { get; set; }
        public string Status { get; set; } = "ACTIVE";
    }

    public class FullCustomProtocolDto
    {
        public int CustomProtocolId { get; set; }
        public int? BaseProtocolId { get; set; }
        public int? AppointmentId { get; set; }
        public string? BaseProtocolName { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Status { get; set; } = "ACTIVE";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<CustomProtocolDetailDto> Details { get; set; } = new();
    }

    public class CreateCustomProtocolRequest
    {
        public int? BaseProtocolId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? AppointmentId { get; set; }
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<CustomProtocolDetailDto> Details { get; set; } = new();
    }

    public class UpdatePatientProtocolRequest
    {
        public int ProtocolId { get; set; }
        public bool IsCustom { get; set; }

        public int? AppointmentId { get; set; }
    }
}