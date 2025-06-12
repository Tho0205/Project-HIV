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
    public bool IsApproved { get; set; } = false;
    public string? ImageUrl { get; set; }


    public User Author { get; set; }
    public ICollection<Comment> Comments { get; set; }
}

