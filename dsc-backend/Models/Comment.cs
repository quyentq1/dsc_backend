using System;
using System.Collections.Generic;

namespace dsc_backend.Models;

public partial class Comment
{
    public int CommentId { get; set; }

    public int? UserId { get; set; }

    public int? ActivityId { get; set; }

    public int? TournamentId { get; set; }

    public string? Comment1 { get; set; }

    public int? Star { get; set; }

    public int? Level { get; set; }

    public string? Image { get; set; }

    public virtual Activity? Activity { get; set; }

    public virtual Tournament? Tournament { get; set; }

    public virtual User? User { get; set; }
}
