using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HIV.Models;

public partial class User
{
    public int UserId { get; set; }

    public int AccountId { get; set; }

    public string? FullName { get; set; }

    public string? Phone { get; set; }

    public string? Gender { get; set; }

    public DateOnly? Birthdate { get; set; }

    public string? Role { get; set; }
    
    [JsonIgnore]
    public virtual Account Account { get; set; } = null!;
    
    [JsonIgnore]
    public virtual ICollection<Appointment> PatientAppointments { get; set; } = new List<Appointment>();
    
    [JsonIgnore]
    public virtual ICollection<Appointment> DoctorAppointments { get; set; } = new List<Appointment>();


    public virtual ICollection<Blog> Blogs { get; set; } = new List<Blog>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<CustomizedArvProtocol> CustomizedArvProtocolDoctors { get; set; } = new List<CustomizedArvProtocol>();

    public virtual ICollection<CustomizedArvProtocol> CustomizedArvProtocolPatients { get; set; } = new List<CustomizedArvProtocol>();

    public virtual DoctorInfo? DoctorInfo { get; set; }

    public virtual ICollection<EducationalResource> EducationalResources { get; set; } = new List<EducationalResource>();

    public virtual ICollection<Examination> ExaminationDoctors { get; set; } = new List<Examination>();

    public virtual ICollection<Examination> ExaminationPatients { get; set; } = new List<Examination>();

    public virtual ICollection<Prescription> PrescriptionDoctors { get; set; } = new List<Prescription>();

    public virtual ICollection<Prescription> PrescriptionPatients { get; set; } = new List<Prescription>();

    public virtual ICollection<Reminder> Reminders { get; set; } = new List<Reminder>();
    [JsonIgnore]
    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
