using System;
using System.Collections.Generic;

namespace dsc_backend.Models;

public partial class Club
{
    public int ClubId { get; set; }

    public int? SportId { get; set; }

    public int? UserId { get; set; }

    public int? LevelId { get; set; }

    public int? MemberId { get; set; }

    public string? ClubName { get; set; }

    public string? Status { get; set; }

    public string? Rules { get; set; }

    public DateTime? CreateDate { get; set; }

    public virtual Level? Level { get; set; }

    public virtual ICollection<MemberClub> MemberClubs { get; set; } = new List<MemberClub>();

    public virtual Sport? Sport { get; set; }

    public virtual User? User { get; set; }

    public virtual ICollection<UserClub> UserClubs { get; set; } = new List<UserClub>();
}
