using System;
using System.Collections.Generic;

namespace HIV.Models;

public partial class EducationalResource
{
    [System.ComponentModel.DataAnnotations.Key]
    public int ResourceId { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User? CreatedByNavigation { get; set; }
}
