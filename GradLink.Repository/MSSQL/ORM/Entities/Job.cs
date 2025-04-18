using System;
using System.Collections.Generic;

namespace GradLink.Repository.MSSQL.ORM.Entities;

public partial class Job
{
    public int JobId { get; set; }

    public string Title { get; set; } = null!;

    public string? Location { get; set; }

    public string? ToEmail { get; set; }

    public string? Company { get; set; }

    public string? Description { get; set; }

    public DateTime? PostedDate { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<JobApplication> JobApplications { get; set; } = new List<JobApplication>();
}
