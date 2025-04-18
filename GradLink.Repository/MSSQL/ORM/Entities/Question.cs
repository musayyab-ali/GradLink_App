using System;
using System.Collections.Generic;

namespace GradLink.Repository.MSSQL.ORM.Entities;

public partial class Question
{
    public int Id { get; set; }

    public string UserName { get; set; } = null!;

    public string QuestionText { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();
}
