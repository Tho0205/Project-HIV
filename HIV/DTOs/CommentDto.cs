namespace HIV.DTOs
{
    public class CommentDto
    {
        public int CommentId { get; set; }
        public int BlogId { get; set; }
        public int UserId { get; set; }
        public string? Content { get; set; }
        public string Status { get; set; } = "ACTIVE";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? User { get; set; }
    }
}
