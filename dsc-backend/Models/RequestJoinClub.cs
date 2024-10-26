using System;
using System.Collections.Generic;

namespace dsc_backend.Models;

public partial class RequestJoinClub
{
    public int RequestClubId { get; set; }

    public int UserId { get; set; }

    public int ClubId { get; set; }

    public string? Status { get; set; }

    public DateTime? Createdate { get; set; }

    public virtual Club? Club { get; set; }

    public virtual User? User { get; set; }
}
