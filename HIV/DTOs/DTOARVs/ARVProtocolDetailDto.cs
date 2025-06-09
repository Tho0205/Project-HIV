using System.ComponentModel.DataAnnotations;

namespace HIV.DTOs.DTOARVs
{
    public class ARVProtocolDetailDto
    {
        public int Id { get; set; }
        public int ProtocolId { get; set; }
        public int ArvId { get; set; }
        public string? Dosage { get; set; }
        public string? UsageInstruction { get; set; }
        public string Status { get; set; } = "ACTIVE";

        // Optional: include names if you want to show related info
        public string? ProtocolName { get; set; }
        public string? ArvName { get; set; }
    }
    public class CreateARVProtocolDetailDto
    {
        [Required]
        public int ProtocolId { get; set; }

        [Required]
        public int ArvId { get; set; }

        public string? Dosage { get; set; }
        public string? UsageInstruction { get; set; }

        public string Status { get; set; } = "ACTIVE";
    }
    public class UpdateARVProtocolDetailDto
    {
        [Required]
        public int ProtocolId { get; set; }

        [Required]
        public int ArvId { get; set; }

        public string? Dosage { get; set; }
        public string? UsageInstruction { get; set; }
        public string Status { get; set; } = "ACTIVE";
    }
}