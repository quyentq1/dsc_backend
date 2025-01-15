using System;
using System.Collections.Generic;

namespace dsc_backend.Models;

public partial class CommentClub
{
    public int CommentClubId { get; set; }

    public int? UserId { get; set; }

    public int? ActivityClubId { get; set; }

    public string? Comment { get; set; }

    public virtual ActivitiesClub? ActivityClub { get; set; }

    public virtual User? User { get; set; }
}
