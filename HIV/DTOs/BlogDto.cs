namespace HIV.DTOs
{
    public class BlogDto
    {
        public int BlogId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int AuthorId { get; set; } // dùng khi gửi lên từ client

        public string? Author { get; set; } // dùng khi trả dữ liệu về

        public DateTime CreatedAt { get; set; }
        public bool IsApproved { get; set; } = false;
    }



}
