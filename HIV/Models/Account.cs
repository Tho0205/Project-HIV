using System;
using System.Collections.Generic;

namespace HIV.Models;

public partial class Account
{
    public int AccountId { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? Email { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User? UserTable { get; set; }
}

    //status NVARCHAR(20) DEFAULT 'ACTIVE' CHECK(status IN ('ACTIVE', 'INACTIVE', 'DELETED')),
    //updated_at DATETIME DEFAULT GETDATE()