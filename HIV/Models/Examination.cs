using System;
using System.Collections.Generic;

namespace HIV.Models;

public partial class Examination
{
    [System.ComponentModel.DataAnnotations.Key]
    public int ExamId { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public string? Result { get; set; }
    public int? Cd4Count { get; set; }
    public int? HivLoad { get; set; }
    public DateOnly? ExamDate { get; set; }
    public string Status { get; set; } = "ACTIVE";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int? AppointmentId { get; set; }
    public Appointment Appointment { get; set; }
    public User Patient { get; set; }
    public User Doctor { get; set; }

    public MedicalRecord MedicalRecord { get; set; }
}

//Trong bằng này không có PrescriptionId vì nó không phải là bảng con của Prescription.Nó chỉ liên kết với Prescription thông qua ExamId
//    .status NVARCHAR(20) DEFAULT 'ACTIVE' CHECK(status IN ('ACTIVE', 'INACTIVE', 'DELETED', 'ARCHIVED')),
//    created_at DATETIME DEFAULT GETDATE(),
//    updated_at DATETIME DEFAULT GETDATE(),
