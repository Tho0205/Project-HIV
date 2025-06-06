using System;
using System.Collections.Generic;

namespace HIV.Models;

public partial class CustomizedArvProtocol
{
    [System.ComponentModel.DataAnnotations.Key]
    public int CustomProtocolId { get; set; }
    public int? DoctorId { get; set; }
    public int? PatientId { get; set; }
    public int? BaseProtocolId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; } = "ACTIVE";

    public User Doctor { get; set; }
    public User Patient { get; set; }
    public ARVProtocol BaseProtocol { get; set; }

    public ICollection<CustomizedArvProtocolDetail> Details { get; set; }
    public ICollection<MedicalRecord> MedicalRecords { get; set; }
}

//Trong bảng này không có base_protocolId vì nó không phải là bảng con của BaseArvProtocol.Nó chỉ liên kết với BaseArvProtocol thông qua BaseArvProtocolId trong CustomizedArvProtocolDetail.
//    status NVARCHAR(20) DEFAULT 'ACTIVE' CHECK(status IN ('ACTIVE', 'INACTIVE', 'DELETED', 'COMPLETED')),
//    updated_at DATETIME DEFAULT GETDATE(),