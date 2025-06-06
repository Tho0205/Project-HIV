using System;
using System.Collections.Generic;

namespace HIV.Models;

public partial class Comment
{
    [System.ComponentModel.DataAnnotations.Key]
    public int CommentId { get; set; }
    public int BlogId { get; set; }
    public int UserId { get; set; }
    public string? Content { get; set; }
    public string Status { get; set; } = "ACTIVE";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Blog Blog { get; set; }
    public User User { get; set; }
}

