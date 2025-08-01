using System.ComponentModel.DataAnnotations;

namespace HIV.Models
{
    public class MedicalRecord
    {
        [Key]
        public int RecordId { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int ExamId { get; set; }
        public int AppointmentId { get; set; }
        public int? CustomProtocolId { get; set; }
        public DateTime? ExamDate { get; set; }
        public TimeSpan? ExamTime { get; set; }
        public string Status { get; set; } = "ACTIVE";
        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
        public string? Summary { get; set; }

        public User Patient { get; set; }
        public User Doctor { get; set; }
        public Examination Examination { get; set; }
        public CustomizedArvProtocol CustomProtocol { get; set; }

        public virtual Appointment Appointment { get; set; }
    }
}
