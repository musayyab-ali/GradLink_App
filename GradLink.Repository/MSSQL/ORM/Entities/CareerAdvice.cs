using System;
using System.Collections.Generic;

namespace GradLink.Repository.MSSQL.ORM.Entities;

public partial class CareerAdvice
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime CreatedDate { get; set; }
}
