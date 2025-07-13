using HIV.DTOs;
using HIV.DTOs.DTOSchedule;
using HIV.Models;

namespace HIV.Mappers
{
    public static class UserMapper
    {
        public static UserTableDTO ToUserTableDTO(this User dto)
        {
            return new UserTableDTO
            {
                UserId = dto.UserId,
                AccountId = dto.AccountId,
                FullName = dto.FullName,
                Phone = dto.Phone,
                Gender = dto.Gender,
                Birthdate = dto.Birthdate,
                Role = dto.Role,
                UserAvatar = dto.UserAvatar,
                //Appointments = dto.Appointments,
                Schedules = dto.Schedules.Select(s => new ScheduleSimpleDTO
                {
                    ScheduleId = s.ScheduleId,
                    ScheduledTime = s.ScheduledTime,
                    Room = s.Room
                }).ToList()
            };
        }
    }
}
