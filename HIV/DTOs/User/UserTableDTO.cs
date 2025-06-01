using HIV.Models;
using HIV.DTOs.DTOSchedule;
namespace HIV.DTOs
{
    public class UserTableDTO
    {
        public int UserId { get; set; }

        public int AccountId { get; set; }

        public string? FullName { get; set; }

        public string? Phone { get; set; }

        public string? Gender { get; set; }

        public DateOnly? Birthdate { get; set; }

        public string? Role { get; set; }

        public virtual List<ScheduleSimpleDTO>? Schedules { get; set; }

    }
}
