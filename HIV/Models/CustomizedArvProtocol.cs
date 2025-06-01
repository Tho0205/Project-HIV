using System;
using System.Collections.Generic;

namespace HIV.Models;

public partial class CustomizedArvProtocol
{
    public int CustomProtocolId { get; set; }

    public int? DoctorId { get; set; }

    public int? PatientId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<CustomizedArvProtocolDetail> CustomizedArvProtocolDetails { get; set; } = new List<CustomizedArvProtocolDetail>();

    public virtual User? Doctor { get; set; }

    public virtual User? Patient { get; set; }

    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
}

//Trong bảng này không có base_protocolId vì nó không phải là bảng con của BaseArvProtocol.Nó chỉ liên kết với BaseArvProtocol thông qua BaseArvProtocolId trong CustomizedArvProtocolDetail.
//    status NVARCHAR(20) DEFAULT 'ACTIVE' CHECK(status IN ('ACTIVE', 'INACTIVE', 'DELETED', 'COMPLETED')),
//    updated_at DATETIME DEFAULT GETDATE(),