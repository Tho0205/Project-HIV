using System.ComponentModel.DataAnnotations;

namespace HIV.DTOs.DTOAppointment
{
    public class UpdateAppointmentStatusDTO
    {
        [Required]
        public int AppointmentId { get; set; }

        [Required]
        //[RegularExpression("^(COMPLETED|CANCELLED|CONFIRMED)$",
        //    ErrorMessage = "Status must be COMPLETED, CANCELLED, or CONFIRMED")]
        public string Status { get; set; }

        public string? Note { get; set; }
    }
}