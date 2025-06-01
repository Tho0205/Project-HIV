using HIV.DTOs.DTOAppointment;

namespace HIV.Mappers
{
    public static class AppointmentMapper
    {
        public static AppointmentDTO ToAppointmentDTO(this CreateAppointmentDTO dto)
        {
            return new AppointmentDTO
            {
                ScheduleId = dto.ScheduleId,
                PatientId = dto.PatientId,
                doctorId = dto.doctorId,
                Note = dto.Note,
                AppointmentDate = dto.AppointmentDate
            };
        }
    }
}
