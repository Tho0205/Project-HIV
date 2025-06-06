using System;
using System.Collections.Generic;

namespace HIV.Models;

public partial class Schedule
{
    [System.ComponentModel.DataAnnotations.Key]
    public int ScheduleId { get; set; }
    public int DoctorId { get; set; }
    public DateTime ScheduledTime { get; set; }
    public string? Room { get; set; }
    public string Status { get; set; } = "ACTIVE";

    public User Doctor { get; set; }
    public ICollection<Appointment> Appointments { get; set; }
}
