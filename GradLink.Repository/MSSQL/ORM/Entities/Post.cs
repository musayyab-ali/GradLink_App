using System;
using System.Collections.Generic;

namespace GradLink.Repository.MSSQL.ORM.Entities;

public partial class Post
{
    public int PostId { get; set; }

    public int UserId { get; set; }

    public string? TextContent { get; set; }

    public string? ImagePath { get; set; }

    public DateTime? CreatedOn { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

    public virtual User User { get; set; } = null!;
}
