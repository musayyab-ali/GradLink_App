using System;
using System.Collections.Generic;

namespace GradLink.Repository.MSSQL.ORM.Entities;

public partial class Answer
{
    public int Id { get; set; }

    public int? QuestionId { get; set; }

    public string AnsweredBy { get; set; } = null!;

    public string AnswerText { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public virtual Question? Question { get; set; }
}
