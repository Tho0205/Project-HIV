namespace HIV.DTOs.DTOARVs
{
    public class CustomizedArvProtocolDto
    {
        public int CustomProtocolId { get; set; }
        public int? DoctorId { get; set; }
        public int? PatientId { get; set; }
        public int? BaseProtocolId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = "ACTIVE";
    }
    public class CreateCustomizedArvProtocolDto
    {
        public int? DoctorId { get; set; }
        public int? PatientId { get; set; }
        public int? BaseProtocolId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = "ACTIVE";
    }
    public class UpdateCustomizedArvProtocolDto
    {
        public int CustomProtocolId { get; set; }
        public int? DoctorId { get; set; }
        public int? PatientId { get; set; }
        public int? BaseProtocolId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = "ACTIVE";
    }

}
