using System;
using System.Collections.Generic;

namespace dsc_backend.Models;

public partial class UserActivityClub
{
    public int UserActivityClubId { get; set; }

    public int UserId { get; set; }

    public int ActivityId { get; set; }

    public int ClubId { get; set; }

    public DateTime? JoinDate { get; set; }

    public string? Role { get; set; }

    public string? Status { get; set; }

    public virtual Activity Activity { get; set; } = null!;

    public virtual Club Club { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
