using System;
using System.Collections.Generic;

namespace HIV.Models;

public partial class CustomizedArvProtocol
{
    [System.ComponentModel.DataAnnotations.Key]
    public int CustomProtocolId { get; set; }
    public int? DoctorId { get; set; }
    public int? PatientId { get; set; }
    public int? BaseProtocolId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; } = "ACTIVE";
    public int? AppointmentId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public User Doctor { get; set; }
    public User Patient { get; set; }
    public ARVProtocol BaseProtocol { get; set; }
    public Appointment Appointment { get; set; }
    public ICollection<CustomizedArvProtocolDetail> Details { get; set; }
    public ICollection<MedicalRecord> MedicalRecords { get; set; }
}

