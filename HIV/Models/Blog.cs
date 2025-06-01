using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Principal;

namespace HIV.Models;

public partial class Blog
{
    public int BlogId { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public int? AuthorId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User? Author { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
}





    //status NVARCHAR(20) DEFAULT 'ACTIVE' CHECK(status IN ('ACTIVE', 'INACTIVE', 'DELETED', 'DRAFT', 'PUBLISHED')),
    //updated_at DATETIME DEFAULT GETDATE(),
    //published_at DATETIME NULL,
