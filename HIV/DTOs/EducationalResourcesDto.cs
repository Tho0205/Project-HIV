namespace HIV.Interfaces
{
    public class EducationalResourcesDto
    {
        public int ResourceId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
    }
}
