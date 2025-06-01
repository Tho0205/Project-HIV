using System;
using System.Collections.Generic;

namespace HIV.Models;

public partial class Examination
{
    public int ExamId { get; set; }

    public int? PatientId { get; set; }

    public int? DoctorId { get; set; }

    public string? Result { get; set; }

    public int? Cd4Count { get; set; }

    public int? HivLoad { get; set; }

    public DateOnly? ExamDate { get; set; }

    public virtual User? Doctor { get; set; }

    public virtual User? Patient { get; set; }

    public virtual Prescription? Prescription { get; set; }
}

//Trong bằng này không có PrescriptionId vì nó không phải là bảng con của Prescription.Nó chỉ liên kết với Prescription thông qua ExamId
//    .status NVARCHAR(20) DEFAULT 'ACTIVE' CHECK(status IN ('ACTIVE', 'INACTIVE', 'DELETED', 'ARCHIVED')),
//    created_at DATETIME DEFAULT GETDATE(),
//    updated_at DATETIME DEFAULT GETDATE(),
