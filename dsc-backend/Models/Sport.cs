using System;
using System.Collections.Generic;

namespace dsc_backend.Models;

public partial class Sport
{
    public int SportId { get; set; }

    public string? SportName { get; set; }

    public string? Avatar { get; set; }

    public virtual ICollection<Club> Clubs { get; set; } = new List<Club>();

    public virtual ICollection<UserSport> UserSports { get; set; } = new List<UserSport>();
}
