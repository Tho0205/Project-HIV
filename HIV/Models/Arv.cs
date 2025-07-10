using System;
using System.Collections.Generic;

namespace HIV.Models;

public partial class Arv
{
    [System.ComponentModel.DataAnnotations.Key]
    public int ArvId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; } = "ACTIVE";

    public ICollection<ARVProtocolDetail> ProtocolDetails { get; set; }
    public ICollection<CustomizedArvProtocolDetail> CustomProtocolDetails { get; set; }
}
