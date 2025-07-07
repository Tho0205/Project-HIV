using System;
using System.Collections.Generic;

namespace HIV.Models;

public partial class CustomizedArvProtocolDetail
{
    [System.ComponentModel.DataAnnotations.Key]
    public int Id { get; set; }
    public int CustomProtocolId { get; set; }
    public int ArvId { get; set; }
    public string? Dosage { get; set; }
    public string? UsageInstruction { get; set; }
    public string Status { get; set; } = "ACTIVE";

    public CustomizedArvProtocol CustomProtocol { get; set; }
    public Arv Arv { get; set; }
}

