using System;
using System.Collections.Generic;

namespace HIV.Models;

public partial class Prescription
{
    public int PrescriptionId { get; set; }

    public int PatientId { get; set; }

    public int DoctorId { get; set; }

    public int ExaminationId { get; set; }

    public int? CustomProtocolId { get; set; }

    public DateOnly? ExamDate { get; set; }

    public TimeOnly? ExamTime { get; set; }

    public DateTime? IssuedAt { get; set; }

    public virtual CustomizedArvProtocol? CustomProtocol { get; set; }

    public virtual User Doctor { get; set; } = null!;

    public virtual Examination Examination { get; set; } = null!;

    public virtual User Patient { get; set; } = null!;
}
