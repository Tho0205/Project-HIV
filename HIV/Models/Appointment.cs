using System;
using System.Collections.Generic;

namespace HIV.Models;

public partial class Appointment
{
    [System.ComponentModel.DataAnnotations.Key]
    public int AppointmentId { get; set; }
    public int? PatientId { get; set; }
    public int ScheduleId { get; set; }
    public int DoctorId { get; set; }
    public string? Note { get; set; }
    public string? AppoinmentType { get; set; }
    public bool IsAnonymous { get; set; } = false;
    public string Status { get; set; } = "SCHEDULED";
    public DateTime AppointmentDate { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User Patient { get; set; }
    public User Doctor { get; set; }
    public Schedule Schedule { get; set; }

}
