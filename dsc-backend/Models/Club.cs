using System;
using System.Collections.Generic;

namespace dsc_backend.Models;

public partial class Club
{
    public int ClubId { get; set; }

    public int? SportId { get; set; }

    public int? LevelId { get; set; }

    public string? ClubName { get; set; }

    public string? Status { get; set; }

    public string? Rules { get; set; }

    public DateTime? CreateDate { get; set; }

    public decimal? Fund { get; set; }

    public virtual Level? Level { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<RequestJoinClub> RequestJoinClubs { get; set; } = new List<RequestJoinClub>();

    public virtual Sport? Sport { get; set; }

    public virtual ICollection<TransferHistory> TransferHistoryReceiverClubs { get; set; } = new List<TransferHistory>();

    public virtual ICollection<TransferHistory> TransferHistorySenderClubs { get; set; } = new List<TransferHistory>();

    public virtual ICollection<UserClub> UserClubs { get; set; } = new List<UserClub>();
}
