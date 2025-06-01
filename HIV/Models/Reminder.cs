using System;
using System.Collections.Generic;

namespace HIV.Models;

public partial class Reminder
{
    public int ReminderId { get; set; }

    public int? UserId { get; set; }

    public string? Content { get; set; }

    public DateTime? RemindAt { get; set; }

    public string? Type { get; set; }

    public virtual User? User { get; set; }
}

    //status NVARCHAR(20) DEFAULT 'ACTIVE' CHECK(status IN ('ACTIVE', 'COMPLETED', 'CANCELLED', 'DELETED')),
    //created_at DATETIME DEFAULT GETDATE(),
    //updated_at DATETIME DEFAULT GETDATE(),