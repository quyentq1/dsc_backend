using System;
using System.Collections.Generic;

namespace dsc_backend.Models;

public partial class MemberClub
{
    public int MemberId { get; set; }

    public int ClubId { get; set; }

    public int? UserId { get; set; }

    public DateTime? JoinDate { get; set; }

    public virtual Club Club { get; set; } = null!;

    public virtual User? User { get; set; }
}
