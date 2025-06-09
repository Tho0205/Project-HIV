namespace HIV.DTOs.DTOARVs
{
    public class ArvDto
    {
        public int ArvId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = "ACTIVE";
    }

    public class CreateArvDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = "ACTIVE";
    }

    public class UpdateArvDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; }
    }
}
                