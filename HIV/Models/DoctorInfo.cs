using System;
using System.Collections.Generic;

namespace HIV.Models;

public partial class DoctorInfo
{
    public int DoctorId { get; set; }

    public string? Degree { get; set; }

    public string? Specialization { get; set; }

    public int? ExperienceYears { get; set; }

    public virtual User Doctor { get; set; } = null!;
}
  //  status NVARCHAR(20) DEFAULT 'ACTIVE' CHECK(status IN ('ACTIVE', 'INACTIVE', 'DELETED', 'ON_LEAVE')),
  //updated_at DATETIME DEFAULT GETDATE(),