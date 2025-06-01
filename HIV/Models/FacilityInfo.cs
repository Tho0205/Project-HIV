using System;
using System.Collections.Generic;

namespace HIV.Models;

public partial class FacilityInfo
{
    public int FacilityId { get; set; }

    public string? Name { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Description { get; set; }
}

    //status NVARCHAR(20) DEFAULT 'ACTIVE' CHECK(status IN ('ACTIVE', 'INACTIVE', 'DELETED')),
    //created_at DATETIME DEFAULT GETDATE(),
    //updated_at DATETIME DEFAULT GETDATE()