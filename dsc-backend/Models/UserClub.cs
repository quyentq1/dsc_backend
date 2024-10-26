using System;
using System.Collections.Generic;

namespace dsc_backend.Models;

public partial class UserClub
{
    public int UserClubId { get; set; }

    public int? ClubId { get; set; }

    public int? UserId { get; set; }

    public string? Role { get; set; }

    public DateTime? JoinDate { get; set; }

    public int? Status { get; set; }

    public virtual Club? Club { get; set; }

    public virtual User? User { get; set; }
}
