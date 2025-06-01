using System;
using System.Collections.Generic;

namespace HIV.Models;

public partial class EducationalResource
{
    public int ResourceId { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User? CreatedByNavigation { get; set; }
}
   // status NVARCHAR(20) DEFAULT 'ACTIVE' CHECK(status IN ('ACTIVE', 'INACTIVE', 'DELETED', 'ARCHIVED')),
   //updated_at DATETIME DEFAULT GETDATE(),