using System.ComponentModel.DataAnnotations;

namespace HIV.Models
{
    public class ARVProtocolDetail
    {
        [Key]
        public int Id { get; set; }
        public int ProtocolId { get; set; }
        public int ArvId { get; set; }
        public string? Dosage { get; set; }
        public string? UsageInstruction { get; set; }
        public string Status { get; set; } = "ACTIVE";

        public ARVProtocol Protocol { get; set; }
        public Arv Arv { get; set; }
    }
}
