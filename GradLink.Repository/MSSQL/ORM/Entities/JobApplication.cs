using System;
using System.Collections.Generic;

namespace GradLink.Repository.MSSQL.ORM.Entities;

public partial class JobApplication
{
    public int ApplicationId { get; set; }

    public int? JobId { get; set; }

    public int? UserId { get; set; }

    public string? Email { get; set; }

    public string? CvPath { get; set; }

    public DateTime? AppliedOn { get; set; }

    public virtual Job? Job { get; set; }

    public virtual User? User { get; set; }
}
