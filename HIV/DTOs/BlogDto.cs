namespace HIV.DTOs
{
    public class BlogDto
    {
        public int BlogId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int AuthorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsApproved { get; set; } = false;
        public string? ImageUrl { get; set; }


        public string? Author { get; set; } 


    }



}
