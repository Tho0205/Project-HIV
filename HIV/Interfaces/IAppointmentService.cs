using HIV.DTOs;
using HIV.DTOs.DTOAppointment;
using HIV.DTOs.DTOSchedule;

namespace HIV.Interfaces
{
    public interface IAppointmentService
    {
        Task<List<UserTableDTO>> GetAllListDoctor();
        Task<List<ScheduleSimpleDTO>> GetScheduleOfDoctor(int id_doctor);
        Task<CreateAppointmentDTO> CreateAppointment(CreateAppointmentDTO dto);
        Task<UserTableDTO> GetInforUser(int id);
        Task<bool> CancelAppointment(int id);
        Task<List<AppointmentDTO>> GetAll();
        Task<List<PatientOfDoctorDTO>> GetPatientsOfDoctor(int doctorId);

        Task<bool> UpdateAppointmentStatus(UpdateAppointmentStatusDTO dto);
        Task<bool> ConfirmAppointment(int appointmentId, string? note = null);
        Task<bool> CompleteAppointment(int appointmentId, string? note = null);
        Task<AppointmentDTO> GetAppointmentById(int appointmentId);
        Task<List<ScheduleSimpleDTO>> GetAllScheduleOfDoctor(int id_doctor);
        Task CreateRelatedRecordsAsync(int appointmentId);
    }
}