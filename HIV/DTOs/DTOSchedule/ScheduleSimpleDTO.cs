namespace HIV.DTOs.DTOSchedule
{
    public class ScheduleSimpleDTO
    {
        public int ScheduleId { get; set; }
        public DateTime ScheduledTime { get; set; }
        public string? Room { get; set; }
        public string? Status { get; set; } // Thêm Status
    }
}
