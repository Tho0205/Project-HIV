using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Principal;

namespace HIV.Models;

public partial class Blog
{
    [System.ComponentModel.DataAnnotations.Key]
    public int BlogId { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public int? AuthorId { get; set; }
    public string Status { get; set; } = "ACTIVE";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PublishedAt { get; set; }

    public User Author { get; set; }
    public ICollection<Comment> Comments { get; set; }
}





    //status NVARCHAR(20) DEFAULT 'ACTIVE' CHECK(status IN ('ACTIVE', 'INACTIVE', 'DELETED', 'DRAFT', 'PUBLISHED')),
    //updated_at DATETIME DEFAULT GETDATE(),
    //published_at DATETIME NULL,
