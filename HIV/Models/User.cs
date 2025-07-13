using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HIV.Models;

public partial class User
{
    [System.ComponentModel.DataAnnotations.Key]
    public int UserId { get; set; }
    public int AccountId { get; set; }
    public string? FullName { get; set; }
    public string? Phone { get; set; }
    public string? UserAvatar { get; set; }
    public string? Address { get; set; }
    public string? Gender { get; set; }
    public DateOnly? Birthdate { get; set; }
    public string? Role { get; set; }
    public string Status { get; set; } = "ACTIVE";

    public Account Account { get; set; }

    public DoctorInfo DoctorInfo { get; set; }

    public ICollection<Blog> Blogs { get; set; }
    public ICollection<Comment> Comments { get; set; }
    public ICollection<EducationalResource> EducationalResources { get; set; }
    public ICollection<Schedule> Schedules { get; set; }

    public ICollection<Appointment> AppointmentsAsPatient { get; set; }
    public ICollection<Appointment> AppointmentsAsDoctor { get; set; }

    public ICollection<Examination> ExaminationsAsPatient { get; set; }
    public ICollection<Examination> ExaminationsAsDoctor { get; set; }

    public ICollection<CustomizedArvProtocol> CustomProtocolsAsDoctor { get; set; }
    public ICollection<CustomizedArvProtocol> CustomProtocolsAsPatient { get; set; }

    public ICollection<MedicalRecord> MedicalRecordsAsDoctor { get; set; }
    public ICollection<MedicalRecord> MedicalRecordsAsPatient { get; set; }
    public ICollection<Notification> Notifications { get; set; }

}
