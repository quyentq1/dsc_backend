using System;
using System.Collections.Generic;

namespace dsc_backend.Models;

public partial class Round
{
    public int RoundId { get; set; }

    public int? TournamentId { get; set; }

    public int? RoundNumber { get; set; }

    public virtual ICollection<Match> Matches { get; set; } = new List<Match>();

    public virtual Tournament? Tournament { get; set; }
}
