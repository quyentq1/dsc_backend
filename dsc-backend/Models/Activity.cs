using System;
using System.Collections.Generic;

namespace dsc_backend.Models;

public partial class Activity
{
    public int ActivityId { get; set; }

    public int? UserId { get; set; }

    public int? LevelId { get; set; }

    public string? ActivityName { get; set; }

    public DateTime? StartDate { get; set; }

    public string? Location { get; set; }

    public int? NumberOfTeams { get; set; }

    public decimal? Expense { get; set; }

    public string? Description { get; set; }

    public string? Avatar { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual Level? Level { get; set; }

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<RequestJoinActivity> RequestJoinActivities { get; set; } = new List<RequestJoinActivity>();

    public virtual ICollection<ResultOfActivity> ResultOfActivities { get; set; } = new List<ResultOfActivity>();

    public virtual User? User { get; set; }

    public virtual ICollection<UserActivity> UserActivities { get; set; } = new List<UserActivity>();
}
