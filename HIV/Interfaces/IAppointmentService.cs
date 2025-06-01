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
    }
}
