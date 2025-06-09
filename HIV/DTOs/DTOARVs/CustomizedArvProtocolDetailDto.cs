namespace HIV.DTOs.DTOARVs
{
    public class CustomizedArvProtocolDetailDto
    {
        public int Id { get; set; }
        public int CustomProtocolId { get; set; }
        public int ArvId { get; set; }
        public string? Dosage { get; set; }
        public string? UsageInstruction { get; set; }
        public string Status { get; set; } = "ACTIVE";
    }
    public class CreateCustomizedArvProtocolDetailDto
    {
        public int CustomProtocolId { get; set; }
        public int ArvId { get; set; }
        public string? Dosage { get; set; }
        public string? UsageInstruction { get; set; }
        public string Status { get; set; } = "ACTIVE";
    }
    public class UpdateCustomizedArvProtocolDetailDto
    {
        public string? Dosage { get; set; }
        public string? UsageInstruction { get; set; }
        public string Status { get; set; }
    }
}
