using System;
using System.Collections.Generic;

namespace GradLink.Repository.MSSQL.ORM.Entities;

public partial class NewsletterSubscription
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public DateTime DateSubscribed { get; set; }
}
