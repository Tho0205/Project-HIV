using System;
using System.Collections.Generic;

namespace HIV.Models;

public partial class Appointment
{
    public int AppointmentId { get; set; }

    public int PatientId { get; set; }

    public int ScheduleId { get; set; }

    public string? Note { get; set; }

    public bool? IsAnonymous { get; set; }

    public string? Status { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime AppointmentDate { get; set; }

    public int DoctorId { get; set; }


    public virtual User? Patient { get; set; }

    public virtual Schedule? Schedule { get; set; }

    public virtual User Doctor { get; set; } = null!; // ← Thêm navigation property

}

    //updated_at DATETIME DEFAULT GETDATE(),