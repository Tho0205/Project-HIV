using System;
using System.Collections.Generic;

namespace HIV.Models;

public partial class Schedule
{
    public int ScheduleId { get; set; }

    public int? DoctorId { get; set; }

    public DateTime ScheduledTime { get; set; }

    public string? Room { get; set; }

    public string? Status { get; set; } 

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual User? Doctor { get; set; }
}

    //status NVARCHAR(20) DEFAULT 'ACTIVE' CHECK(status IN ('ACTIVE', 'INACTIVE', 'DELETED', 'CANCELLED', 'COMPLETED')),
    //    created_at DATETIME DEFAULT GETDATE(),
    //updated_at DATETIME DEFAULT GETDATE(),

    //Bảng này không có khóa ngoại ngoại ApointmentId vì nó không phải là bảng con của Appointment.Nó chỉ liên kết với Appointment thông qua ScheduleId.