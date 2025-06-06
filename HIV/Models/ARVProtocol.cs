using System.ComponentModel.DataAnnotations;

namespace HIV.Models
{
    public class ARVProtocol
    {
        [Key]
        public int ProtocolId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = "ACTIVE";

        public ICollection<ARVProtocolDetail> Details { get; set; }
        public ICollection<CustomizedArvProtocol> CustomizedProtocols { get; set; }
    }
}
