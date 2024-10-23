using System;
using System.Collections.Generic;

namespace dsc_backend.Models;

public partial class Fee
{
    public int FeeId { get; set; }

    public int? TournamentId { get; set; }

    public decimal? RequestFee { get; set; }

    public virtual Tournament? Tournament { get; set; }
}
