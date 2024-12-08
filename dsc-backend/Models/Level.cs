using System;
using System.Collections.Generic;

namespace dsc_backend.Models;

public partial class Level
{
    public int LevelId { get; set; }

    public string? LevelName { get; set; }

    public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();

    public virtual ICollection<ActivitiesClub> ActivitiesClubs { get; set; } = new List<ActivitiesClub>();

    public virtual ICollection<Club> Clubs { get; set; } = new List<Club>();

    public virtual ICollection<Tournament> Tournaments { get; set; } = new List<Tournament>();

    public virtual ICollection<UserSport> UserSports { get; set; } = new List<UserSport>();
}
