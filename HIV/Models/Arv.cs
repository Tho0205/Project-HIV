using System;
using System.Collections.Generic;

namespace HIV.Models;

public partial class Arv
{
    public int ArvId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<CustomizedArvProtocolDetail> CustomizedArvProtocolDetails { get; set; } = new List<CustomizedArvProtocolDetail>();
}

    //status NVARCHAR(20) DEFAULT 'ACTIVE' CHECK(status IN ('ACTIVE', 'INACTIVE', 'DELETED', 'DISCONTINUED')),
    //created_at DATETIME DEFAULT GETDATE(),
    //updated_at DATETIME DEFAULT GETDATE()