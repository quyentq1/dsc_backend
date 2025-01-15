using System;
using System.Collections.Generic;

namespace dsc_backend.Models;

public partial class ActivitiesClub
{
    public int ActivityClubId { get; set; }

    public int? ClubId { get; set; }

    public int? LevelId { get; set; }

    public string? ActivityName { get; set; }

    public DateTime? StartDate { get; set; }

    public string? Location { get; set; }

    public int? NumberOfTeams { get; set; }

    public decimal? Expense { get; set; }

    public string? Description { get; set; }

    public string? Avatar { get; set; }

    public virtual Club? Club { get; set; }

    public virtual ICollection<CommentClub> CommentClubs { get; set; } = new List<CommentClub>();

    public virtual Level? Level { get; set; }

    public virtual ICollection<ResultOfActivitiesClub> ResultOfActivitiesClubs { get; set; } = new List<ResultOfActivitiesClub>();

    public virtual ICollection<UserActivityClub> UserActivityClubs { get; set; } = new List<UserActivityClub>();
}
