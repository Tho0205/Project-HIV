using System;
using System.Collections.Generic;

namespace HIV.Models;

public partial class Comment
{
    public int CommentId { get; set; }

    public int? BlogId { get; set; }

    public int? UserId { get; set; }

    public string? Content { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Blog? Blog { get; set; }

    public virtual User? User { get; set; }
}
    //status NVARCHAR(20) DEFAULT 'ACTIVE' CHECK(status IN ('ACTIVE', 'INACTIVE', 'DELETED', 'FLAGGED')),
    //updated_at DATETIME DEFAULT GETDATE(),
