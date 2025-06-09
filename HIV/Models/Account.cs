using System;
using System.Collections.Generic;

namespace HIV.Models;

// Xin chao
// This file is part of the HIV project, which is licensed under the GNU General Public License v3.0.
public partial class Account
{
    [System.ComponentModel.DataAnnotations.Key]
    public int AccountId { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? Email { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string Status { get; set; } = "ACTIVE";

    public User User { get; set; }
}



