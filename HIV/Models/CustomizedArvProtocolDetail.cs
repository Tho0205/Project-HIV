using System;
using System.Collections.Generic;

namespace HIV.Models;

public partial class CustomizedArvProtocolDetail
{
    public int Id { get; set; }

    public int CustomProtocolId { get; set; }

    public int ArvId { get; set; }

    public string? Dosage { get; set; }

    public string? UsageInstruction { get; set; }

    public virtual Arv Arv { get; set; } = null!;

    public virtual CustomizedArvProtocol CustomProtocol { get; set; } = null!;
}

    //status NVARCHAR(20) DEFAULT 'ACTIVE' CHECK(status IN ('ACTIVE', 'INACTIVE', 'DELETED')),
    //created_at DATETIME DEFAULT GETDATE(),
    //updated_at DATETIME DEFAULT GETDATE(),