namespace HIV.DTOs.DTOARVs
{
    public class ARVProtocolDto
    {
        public int ProtocolId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; } = "ACTIVE";
        public List<ARVProDetailDto> Details { get; set; } = new List<ARVProDetailDto>();
    }

    public class CreateARVProtocolDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; } = "ACTIVE";
    }

    public class CreateARVProtocolWithDetailsDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; } = "ACTIVE";
        public List<CreateARVProtocolDetailDto> Details { get; set; } = new List<CreateARVProtocolDetailDto>();
    }

    public class CreateARVProtocolDetailDto
    {
        public int ArvId { get; set; }
        public string Dosage { get; set; }
        public string UsageInstruction { get; set; }
        public string Status { get; set; } = "ACTIVE";
    }

    public class UpdateARVProtocolDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; } = "ACTIVE";
    }

    public class UpdateARVProtocolWithDetailsDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; } = "ACTIVE";
        public List<UpdateARVProtocolDetailDto> Details { get; set; } = new List<UpdateARVProtocolDetailDto>();
    }

    public class UpdateARVProtocolDetailDto
    {
        public int DetailId { get; set; }
        public int ArvId { get; set; }
        public string Dosage { get; set; }
        public string UsageInstruction { get; set; }
        public string Status { get; set; } = "ACTIVE";
    }

    public class AddARVToProtocolDto
    {
        public int ArvId { get; set; }
        public string Dosage { get; set; }
        public string UsageInstruction { get; set; }
        public string Status { get; set; } = "ACTIVE";
    }

    public class ARVProDetailDto
    {
        public int DetailId { get; set; }
        public int ArvId { get; set; }
        public string ArvName { get; set; }
        public string Dosage { get; set; }
        public string UsageInstruction { get; set; }
        public string Status { get; set; } = "ACTIVE";
    }

    public class ServiceResult<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

        public static ServiceResult<T> Success(T data, string message = null)
        {
            return new ServiceResult<T>
            {
                IsSuccess = true,
                Data = data,
                Message = message
            };
        }

        public static ServiceResult<T> Failure(string errorMessage)
        {
            return new ServiceResult<T>
            {
                IsSuccess = false,
                Message = errorMessage,
                Errors = new List<string> { errorMessage }
            };
        }

        public static ServiceResult<T> Failure(List<string> errorMessages, string mainMessage = null)
        {
            return new ServiceResult<T>
            {
                IsSuccess = false,
                Message = mainMessage ?? errorMessages.FirstOrDefault(),
                Errors = errorMessages
            };
        }
    }
}