using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HIV.Models;

public partial class DoctorInfo
{
    [Key]
    public int DoctorId { get; set; }
    public string? Degree { get; set; }
    public string? Specialization { get; set; }
    public int? ExperienceYears { get; set; }
    public string? DoctorAvatar { get; set; }
    public string Status { get; set; } = "ACTIVE";

    public User Doctor { get; set; }
}
