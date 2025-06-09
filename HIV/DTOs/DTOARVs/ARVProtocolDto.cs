namespace HIV.DTOs.DTOARVs
{
    public class ARVProtocolDto
    {
        public int ProtocolId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = "ACTIVE";
    }
    public class CreateARVProtocolDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = "ACTIVE";
    }

    public class UpdateARVProtocolDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = "ACTIVE";
    }
}
