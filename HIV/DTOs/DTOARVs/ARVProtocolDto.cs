namespace HIV.DTOs.DTOARVs
{
    public class ARVProtocolDto
    {
        public int ProtocolId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = "ACTIVE";
        // Thêm danh sách chi tiết
        public List<ARVProDetailDto>? Details { get; set; }
    }

    public class CreateARVProtocolDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = "ACTIVE";
    }

    // Thêm DTO mới cho việc tạo protocol kèm chi tiết
    public class CreateARVProtocolWithDetailsDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = "ACTIVE";
        public List<CreateARVProtocolDetailDto> Details { get; set; } = new List<CreateARVProtocolDetailDto>();
    }

    // Thêm DTO cho tạo chi tiết protocol
    public class CreateARVProtocolDetailDto
    {
        public int ArvId { get; set; }
        public string Dosage { get; set; }
        public string? UsageInstruction { get; set; }
        public string Status { get; set; } = "ACTIVE";
    }

    public class UpdateARVProtocolDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = "ACTIVE";
    }

    public class ARVProDetailDto
    {
        public int ArvId { get; set; }
        public string ArvName { get; set; }
        public string Dosage { get; set; }
        public string UsageInstruction { get; set; }
        // Thêm ID chi tiết nếu cần
        public int DetailId { get; set; }
    }

    // Thêm DTO cho kết quả trả về
    public class ServiceResult<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }

        public static ServiceResult<T> Success(T data) => new() { IsSuccess = true, Data = data };
        public static ServiceResult<T> Failure(string error) => new() { IsSuccess = false, Errors = new List<string> { error } };
        public static ServiceResult<T> Failure(List<string> errors) => new() { IsSuccess = false, Errors = errors };
    }
}

